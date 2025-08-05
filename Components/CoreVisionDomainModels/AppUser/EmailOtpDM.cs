using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.AppUser
{
    public class EmailOtpDM : CoreVisionDomainModelBase<int>
    {
        public long OTP { get; set; }
        public DateTime OTPExpiry { get; set; }

        public string EmailId { get; set; }

    }
}
