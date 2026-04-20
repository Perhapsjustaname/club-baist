using Microsoft.EntityFrameworkCore;
using ClubBaist.Components;
using ClubBaist.Services;
using ClubBaist.Data;

var builder = WebApplication.CreateBuilder(args);

// blazor server - need interactive server or buttons/events dont fire
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// scoped = one per user session
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeeTimeService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<BillingService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// need BOTH registrations:
// AddDbContext  = for service classes injected via DI (TeeTimeService, MemberService, etc.)
// AddDbContextFactory = for razor components that open their own short-lived contexts
builder.Services.AddDbContext<ClubBaistDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContextFactory<ClubBaistDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

var app = builder.Build();

// EnsureCreated builds the schema from the model classes on first run
// SeedData.Initialize is a no-op if the db already has data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClubBaistDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
