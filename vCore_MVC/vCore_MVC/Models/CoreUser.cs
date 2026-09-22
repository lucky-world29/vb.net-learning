using System.ComponentModel.DataAnnotations.Schema;
using vCore_MVC.Models;

[Table("CoreUsers")]
public class CoreUser
{
    public int Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public int TenantId { get; set; }
    public int? RoleId { get; set; }

    public byte Status { get; set; }          // ✅ tinyint
    public float IsDeleted { get; set; }      // ✅ real

    public int LockedAttempts { get; set; }   // ✅ REQUIRED (you missed this)

    public float Locked { get; set; }
    public float FLocked { get; set; }
    public float OTPLoked { get; set; }
    public float DeadLocked { get; set; }

    [ForeignKey("RoleId")]
    public CoreRole? Role { get; set; }
}