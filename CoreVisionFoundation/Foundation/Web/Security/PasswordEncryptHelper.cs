using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionFoundation.Foundation.AuthenticationHelper;

namespace CoreVisionFoundation.Foundation.Web.Security
{
    public class PasswordEncryptHelper : Rfc2898Helper, IPasswordEncryptHelper, IEncryptHelper
    {
        public PasswordEncryptHelper(string encryptionKey, string decryptionKey)
            : base(encryptionKey, decryptionKey)
        {
        }
    }
}
