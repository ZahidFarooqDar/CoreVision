using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CoreVisionDomainModels.Enums;

namespace CoreVisionDomainModels.v1.General.License
{
    public class UserTestLicenseDetailsDM : CoreVisionDomainModelBase<int>
    {
        public string? SubscriptionPlanName { get; set; }
        public int TestCountValidity { get; set; }
        public double DiscountInPercentage { get; set; }
        public decimal ActualPaidPrice { get; set; }
        
        [DefaultValue(false)]
        public bool IsSuspended { get; set; }
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CancelAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDateUTC { get; set; }

        [ForeignKey(nameof(TestLicenseType))]
        public int? TestLicenseTypeId { get; set; }
        public virtual TestLicenseTypeDM? TestLicenseType { get; set; }


        [ForeignKey(nameof(ClientUser))]
        public int ClientUserId { get; set; }
        public virtual ClientUserDM ClientUser { get; set; }

        public long RemainingAmount { get; set; }
        public PaymentMethodDM PaymentMethod { get; set; }

        public LicenseStatusDM LicenseStatus { get; set; }

        public virtual ICollection<UserTestInvoiceDM> UserTestInvoices { get; set; }
    }
}
