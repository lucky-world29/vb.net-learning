namespace vCore_MVC.Models
{
    public class CoreBranchSubUnit
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? ShortCode { get; set; }

        public long? BranchId { get; set; }

        public float Status { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public float IsDeleted { get; set; }

        public DateTime? DeletionDate { get; set; }

        public string? DeletedBy { get; set; }

        public int TenantId { get; set; }
    }
}
