using CoreVisionDomainModels.AppUser.Login;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoreVisionDomainModels.Client;
using CoreVisionDomainModels.Foundation.Base;
using CoreVisionDomainModels.Enums;

namespace CoreVisionDomainModels.AppUser
{
    public class ExternalUserDM : CoreVisionDomainModelBase<int>
    {
        public string RefreshToken { get; set; }

        [ForeignKey(nameof(ClientUser))]
        public int ClientUserId { get; set; }

        public ExternalUserTypeDM ExternalUserType { get; set; }
        public virtual ClientUserDM ClientUser { get; set; }

    }
}
