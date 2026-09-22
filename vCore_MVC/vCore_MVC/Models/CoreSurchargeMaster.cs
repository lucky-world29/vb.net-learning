using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreSurchargeMaster")]
    public class CoreSurchargeMaster
    {
        public long Id { get; set; }
        public string ShortCode { get; set; }
        public string Description { get; set; }
        public decimal? SurchargeValue { get; set; }
        public int? SurchargeCharge { get; set; }
        
    }
}
