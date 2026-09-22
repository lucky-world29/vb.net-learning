using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreServiceTypes")]
    public class CoreServiceType
    {
        public int Id { get; set; }   
        public string? Name { get; set; }
    }
}
