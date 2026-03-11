namespace ClubBaist.Models;

// simple audit record, one row per staff/admin action
// dont need anything fancy, just who did what and when
public class AuditLog
{
    public int AuditLogId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string Username { get; set; } = string.Empty;  // who did it
    public string Action { get; set; } = string.Empty;    // e.g. "CheckIn", "SuspendMember"
    public string? TargetType { get; set; }               // "Booking", "MemberProfile", "Application"
    public int? TargetId { get; set; }
    public string? Details { get; set; }                  // freeform description, context
}
