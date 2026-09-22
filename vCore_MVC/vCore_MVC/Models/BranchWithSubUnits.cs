namespace vCore_MVC.Models
{
    public class BranchWithSubUnits
    {
        public CoreBranch Branch { get; set; } = new();

        public List<CoreBranchSubUnit> SubUnits { get; set; } = new();

    }
}
