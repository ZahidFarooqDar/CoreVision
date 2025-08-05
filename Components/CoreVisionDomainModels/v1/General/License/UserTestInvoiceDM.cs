using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreVisionDomainModels.v1.General.License
{
    public class UserTestInvoiceDM : CoreVisionDomainModelBase<int>
    {  
        public double DiscountInPercentage { get; set; }
        public decimal ActualPaidPrice { get; set; }
        public long RemainingAmount { get; set; }

        [ForeignKey(nameof(UserLicenseDetails))]
        public int UserTestLicenseDetailsId { get; set; }
        public virtual UserTestLicenseDetailsDM UserLicenseDetails { get; set; }
    }
}
