using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations;

namespace CoreVisionDomainModels.v1.General.License
{
    public class LicenseTypeDM : CoreVisionDomainModelBase<int>
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        public int ValidityInDays { get; set; }

        [StringLength(150)]
        public string LicenseTypeCode { get; set; }

        [Required]
        [StringLength(150)]
        public string StripePriceId { get; set; }

        [Required]
        public RoleTypeDM ValidFor { get; set; }

        public LicensePlanDM LicensePlan { get; set; }

        public double Amount { get; set; }
    }
}
