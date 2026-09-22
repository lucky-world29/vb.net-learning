using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreBranch")]
    public class CoreBranch
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? ShortCode { get; set; }

        // SQL real = float in C#
        public float Status { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        // SQL real = float
        public float IsDeleted { get; set; }

        public DateTime? DeletionDate { get; set; }

        public string? DeletedBy { get; set; }

        public int TenantId { get; set; }
    }
}
