using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreCommodities")]
    public class CoreCommodity
    {
        [Key]
        public long Id { get; set; }                 
        public string? Name { get; set; } 
    }
}
