using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreRoles")]
    public class CoreRole
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }

        [Column("RoleDescription")]   // 🔥 IMPORTANT
        public string? Description { get; set; }

        public float? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public float? IsDeleted { get; set; }
        public int TenantId { get; set; }
    }
}
