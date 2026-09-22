using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vCore_MVC.Models
{
    [Table("CoreTenantOrganisations")]
    public class CoreTenantOrganisation
    {
        [Key]
        public int TenantId { get; set; }

        public string? OrganisationName { get; set; }

        public string? ShortCode { get; set; }

        public string? Email { get; set; }

        public string? Website { get; set; }

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? AttentionTo { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zipcode { get; set; }

        public string? Country { get; set; }

        public string? Currency { get; set; }

        public string? ROC { get; set; }

        public string? TIN { get; set; }

        public string? SalesTaxNo { get; set; }

        public string? ServiceTaxNo { get; set; }

        public string? ReportFooter { get; set; }

        public string? Remark { get; set; }

        public int? DetentionDays { get; set; }

        public string? IncentiveStartDate { get; set; }

        public string? IncentiveEndDate { get; set; }

        public byte[]? ORGImage { get; set; }

        public bool? Status { get; set; }

        public float IsDeleted { get; set; }
    }
}