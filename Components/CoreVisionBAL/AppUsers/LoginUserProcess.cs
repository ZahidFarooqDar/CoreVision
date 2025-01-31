using AutoMapper;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDAL.Context;
using CoreVisionBAL.Foundation.CommonUtils;
using CoreVisionDomainModels.AppUser.Login;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Interfaces;

namespace CoreVisionBAL.AppUsers
{
    public abstract class LoginUserProcess<T> : CodeVisionBalOdataBase<T>
    {
        #region Properties

        protected readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor
        public LoginUserProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }
        #endregion Constructor

        #region CRUD 

        #region Add Update
        /// <summary>
        /// Add or Update profile picture of an user
        /// </summary>
        /// <param name="targetLoginUser"></param>
        /// <param name="webRootPath"></param>
        /// <param name="postedFile"></param>
        /// <returns>
        /// 
        /// </returns>
        protected async Task<string> AddOrUpdateProfilePictureInDb(LoginUserDM targetLoginUser, string webRootPath, IFormFile postedFile)
        {
            if (targetLoginUser != null)
            {
                var currLogoPath = targetLoginUser.ProfilePicturePath;
                var targetRelativePath = Path.Combine("content\\loginusers\\profile", $"{targetLoginUser.Id}_{Guid.NewGuid()}_original{Path.GetExtension(postedFile.FileName)}");
                var targetPath = Path.Combine(webRootPath, targetRelativePath);
                if (await base.SavePostedFileAtPath(postedFile, targetPath))
                {
                    //Entry Method//
                    //var comp = new ClientCompanyDetailDM() { Id = companyId, CompanyLogoPath = targetRelativePath };
                    //_apiDbContext.ClientCompanyDetails.Attach(comp);
                    //_apiDbContext.Entry(comp).Property(e => e.CompanyLogoPath).IsModified = true;
                    targetLoginUser.ProfilePicturePath = WebExtensions.ConvertFromFilePathToUrl(targetRelativePath);
                    targetLoginUser.LastModifiedBy = _loginUserDetail.LoginId;
                    targetLoginUser.LastModifiedOnUTC = DateTime.UtcNow;
                    if (await _apiDbContext.SaveChangesAsync() > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(currLogoPath))
                        { File.Delete(Path.Combine(webRootPath, currLogoPath)); }
                        return WebExtensions.ConvertFromFilePathToUrl(targetRelativePath);
                    }
                }
            }
            return "";
        }

        #endregion Add Update

        #region Delete
        /// <summary>
        /// Deletes the profile picture of an User
        /// </summary>
        /// <param name="targetLoginUser"></param>
        /// <param name="webRootPath"></param>
        /// <returns>
        /// Returns DeleteResponseRoot
        /// </returns>
        protected async Task<DeleteResponseRoot> DeleteProfilePictureById(LoginUserDM targetLoginUser, string webRootPath)
        {
            if (targetLoginUser != null)
            {
                var currLogoPath = targetLoginUser.ProfilePicturePath;
                targetLoginUser.ProfilePicturePath = "";
                targetLoginUser.LastModifiedBy = _loginUserDetail.LoginId;
                targetLoginUser.LastModifiedOnUTC = DateTime.UtcNow;

                if (await _apiDbContext.SaveChangesAsync() > 0)
                {
                    if (!string.IsNullOrWhiteSpace(currLogoPath))
                    {
                        File.Delete(Path.Combine(webRootPath, currLogoPath));
                        return new DeleteResponseRoot(true);
                    }
                }
            }
            return new DeleteResponseRoot(false, "User or Picture Not found");
        }

        #endregion Delete

        #endregion CRUD

        #region Private Functions
        #endregion Private Functions
    }

}
