namespace vCore_MVC.Models
{
    public class BranchSubUnitVM
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? ShortCode { get; set; }

        public long? BranchId { get; set; }

        public string? BranchName { get; set; }

        public float Status { get; set; }
    }
}
