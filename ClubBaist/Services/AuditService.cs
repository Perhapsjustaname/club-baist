using ClubBaist.Data;
using ClubBaist.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubBaist.Services;

// handles writing audit log entries, keeps the pages clean
// use the factory so short lived context
public class AuditService
{
    private readonly IDbContextFactory<ClubBaistDbContext> _dbFactory;

    public AuditService(IDbContextFactory<ClubBaistDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task LogAsync(string username, string action, string? targetType = null, int? targetId = null, string? details = null)
    {
        await using var db = _dbFactory.CreateDbContext();
        db.AuditLogs.Add(new AuditLog
        {
            Username   = username,
            Action     = action,
            TargetType = targetType,
            TargetId   = targetId,
            Details    = details,
            Timestamp  = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetRecentAsync(int count = 200)
    {
        await using var db = _dbFactory.CreateDbContext();
        return await db.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
