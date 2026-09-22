namespace vCore_MVC.Models
{
    public class Rot
    {
        public int Id { get; set; }

        public required string  RotNumber { get; set; }
        public required DateTime RotDate { get; set; }

        public required string JobType { get; set; }
        public required string ProcessType { get; set; }
        public required string Commodity { get; set; }
        public required string Remarks { get; set; }
    }
}
