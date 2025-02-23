using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionBAL.License
{
    public class LicenseTypeProcess : CoreVisionBalOdataBase<LicenseTypeSM>
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        #endregion Properties

        #region Constructor
        public LicenseTypeProcess(IMapper mapper, ApiDbContext apiDbContext, ILoginUserDetail loginUserDetail) : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata
        /// <summary>
        /// This method gets any FeatureGroup(s) by filtering/sorting the data
        /// </summary>
        /// <returns>LicenseType(s)</returns>
        public override async Task<IQueryable<LicenseTypeSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.LicenseTypes;
            IQueryable<LicenseTypeSM> retSM = await base.MapEntityAsToQuerable<LicenseTypeDM, LicenseTypeSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region --Count--

        /// <summary>
        /// Get FeatureGroup Count in database.
        /// </summary>
        /// <returns>integer response</returns>

        public async Task<int> GetAllLicenseTypeCountResponse()
        {
            int resp = _apiDbContext.LicenseTypes.Count();
            return resp;
        }

        #endregion --Count--

        #region Get All

        /// <summary>
        /// Retrieves all license types from the database and maps them to <see cref="LicenseTypeSM"/> objects.
        /// </summary>
        /// <returns>A list of <see cref="LicenseTypeSM"/> objects representing all license types, or <c>null</c> if none are found.</returns>
        public async Task<List<LicenseTypeSM>> GetAllLicenses()
        {
            var _licenseTypeDb = await _apiDbContext.LicenseTypes.ToListAsync();
            if (_licenseTypeDb == null)
            {
                return null;
            }
            return _mapper.Map<List<LicenseTypeSM>>(_licenseTypeDb);
        }

        #endregion Get All

        #region Get Single
        /// <summary>
        /// This method gets a single FeatureGroup on id
        /// </summary>
        /// <param name="id">FeatureGroup Id</param>
        /// <returns>Single FeatureGroup</returns>
        public async Task<LicenseTypeSM?> GetSingleLicenseDetailById(int id)
        {
            LicenseTypeDM? dm = await _apiDbContext.LicenseTypes.FindAsync(id);
            if (dm != null)
            {
                return _mapper.Map<LicenseTypeSM?>(dm);
            }
            return null;
        }

        /// <summary>
        /// This method gets a single FeatureGroup on id
        /// </summary>
        /// <param name="id">FeatureGroup Id</param>
        /// <returns>Single FeatureGroup</returns>
        public async Task<LicenseTypeSM?> GetSingleFeatureDetailByStripePriceId(string stripeId)
        {
            var dm = await _apiDbContext.LicenseTypes.FirstOrDefaultAsync(x => x.StripePriceId == stripeId);
            if(dm != null)
            {
                return _mapper.Map<LicenseTypeSM?>(dm);
            }
            return null;
        }

        #endregion Get Single

        #region Get Single Extended

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<LicenseTypeSM?> GetLicenseDetailsByUserId(int userId)
        {
            var dm = await _apiDbContext.UserLicenseDetails.Where(x=>x.ClientUserId == userId && x.Status == "active" && x.IsSuspended == false && x.IsCancelled == false).FirstOrDefaultAsync();
            if (dm != null)
            {
                return _mapper.Map<LicenseTypeSM?>(dm);
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newStripePriceId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public async Task<string> GetLicenseNameByStripePriceId(string stripePriceId)
        {
            var existingLicense = await _apiDbContext.LicenseTypes.Where(x => x.StripePriceId == stripePriceId).FirstOrDefaultAsync();
            if(existingLicense != null)
            {
                return existingLicense.Title;
            }
            return null;
        }

        #endregion Get Single Extended


    }
}
