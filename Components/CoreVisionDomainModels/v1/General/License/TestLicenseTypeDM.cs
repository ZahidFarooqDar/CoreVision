using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations;

namespace CoreVisionDomainModels.v1.General.License
{
    public class TestLicenseTypeDM : CoreVisionDomainModelBase<int>
    {        
        public string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        public int TestCountValidity { get; set; }

        [StringLength(150)]
        public string LicenseTypeCode { get; set; }

        public double Amount { get; set; }
    }
}
