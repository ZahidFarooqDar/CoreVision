using AutoMapper;
using CoreVisionBAL.AppUsers;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CoreVisionBAL.License
{
    public class PermissionProcess : CoreVisionBalBase
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        private readonly UserLicenseDetailsProcess _userLicenseDetailsProcess;
        private readonly LicenseTypeProcess _licenseTypeProcess;
        private readonly FeatureProcess _featureProcess;
        #endregion Properties

        #region Constructor
        public PermissionProcess(IMapper mapper, ApiDbContext apiDbContext, ILoginUserDetail loginUserDetail,
            UserLicenseDetailsProcess userLicenseDetailsProcess, LicenseTypeProcess licenseTypeProcess, FeatureProcess featureProcess)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
            _licenseTypeProcess = licenseTypeProcess;
            _featureProcess = featureProcess;
            _licenseTypeProcess = licenseTypeProcess;
            _userLicenseDetailsProcess = userLicenseDetailsProcess;
        }
        #endregion Constructor

        #region Method permissions

        public async Task<BoolResponseRoot> DoesUserHasPermission(int userId, string featureCode)
        {
            var existingLicense = await _userLicenseDetailsProcess.GetActiveUserLicenseDetailsByUserId(userId);
            
            var features = await _featureProcess.GetFeaturesbylicenseId((int)existingLicense.LicenseTypeId);
            bool hasPermission = features.Any(feature => feature.FeatureCode == featureCode);
            if(hasPermission == false)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Access_Denied_Log, $"User with Id : {userId} tried to access non permissible feature", "This feature is not available in your current license. Please upgrade to access it.");
            }
            return new BoolResponseRoot(true, "User has permission for this feature.");
            
        }

        #endregion Method permissions
    }
}
