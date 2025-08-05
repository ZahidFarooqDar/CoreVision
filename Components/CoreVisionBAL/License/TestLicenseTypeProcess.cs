using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionBAL.License
{
    public class TestLicenseTypeProcess : CoreVisionBalOdataBase<TestLicenseTypeSM>
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        #endregion Properties

        #region Constructor
        public TestLicenseTypeProcess(IMapper mapper, ApiDbContext apiDbContext, ILoginUserDetail loginUserDetail) : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata
        /// <summary>
        /// This method gets any FeatureGroup(s) by filtering/sorting the data
        /// </summary>
        /// <returns>LicenseType(s)</returns>
        public override async Task<IQueryable<TestLicenseTypeSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.TestLicenseTypes;
            IQueryable<TestLicenseTypeSM> retSM = await base.MapEntityAsToQuerable<TestLicenseTypeDM, TestLicenseTypeSM>(_mapper, entitySet);
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
            int resp = _apiDbContext.TestLicenseTypes.Count();
            return resp;
        }

        #endregion --Count--

        #region Get All

        /// <summary>
        /// Retrieves all license types from the database and maps them to <see cref="TestLicenseTypeSM"/> objects.
        /// </summary>
        /// <returns>A list of <see cref="TestLicenseTypeSM"/> objects representing all license types, or <c>null</c> if none are found.</returns>
        public async Task<List<TestLicenseTypeSM>> GetAllLicenses()
        {
            var _licenseTypeDb = await _apiDbContext.TestLicenseTypes.ToListAsync();
            if (_licenseTypeDb == null)
            {
                return null;
            }
            return _mapper.Map<List<TestLicenseTypeSM>>(_licenseTypeDb);
        }

        #endregion Get All

        #region Get Single
       
        public async Task<TestLicenseTypeSM?> GetSingleLicenseDetailById(int id)
        {
            TestLicenseTypeDM? dm = await _apiDbContext.TestLicenseTypes.FindAsync(id);
            if (dm != null)
            {
                return _mapper.Map<TestLicenseTypeSM?>(dm);
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
        public async Task<TestLicenseTypeSM?> GetLicenseDetailsByUserId(int userId)
        {
            var dm = await _apiDbContext.UserLicenseDetails.Where(x=>x.ClientUserId == userId && x.Status == "active" && x.IsSuspended == false && x.IsCancelled == false).FirstOrDefaultAsync();
            if (dm != null)
            {
                return _mapper.Map<TestLicenseTypeSM?>(dm);
            }
            return null;
        }


        #endregion Get Single Extended

        #region Add

        /// <summary>
        /// Adds a new license type to the database.
        /// </summary>
        /// <param name="model">The <see cref="TestLicenseTypeSM"/> object containing the license type details.</param>
        /// <returns>The added <see cref="TestLicenseTypeSM"/> object with its generated ID.</returns>
        public async Task<TestLicenseTypeSM> AddLicenseType(TestLicenseTypeSM sm)
        {
            if(sm == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.NoRecord_Log, "Test License Type not found.", "License to add not found");
            }
            var dm = _mapper.Map<TestLicenseTypeDM>(sm);
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;
            await _apiDbContext.TestLicenseTypes.AddAsync(dm);
            await _apiDbContext.SaveChangesAsync();
            return _mapper.Map<TestLicenseTypeSM>(dm);
        }

        #endregion Add

        #region Update

        /// <summary>
        /// Updates an existing license type in the database.
        /// </summary>
        /// <param name="model">The updated <see cref="TestLicenseTypeSM"/> object.</param>
        /// <returns><c>true</c> if update was successful; otherwise, <c>false</c>.</returns>
        public async Task<TestLicenseTypeSM> UpdateLicenseType(TestLicenseTypeSM model)
        {
            var existingEntity = await _apiDbContext.TestLicenseTypes.FindAsync(model.Id);
            if (existingEntity == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.NoRecord_Log, $"Test License Type for id: {model.Id} not found to update.", "License to update not found");
            }

            _mapper.Map(model, existingEntity);
            existingEntity.LastModifiedBy = _loginUserDetail.LoginId;
            existingEntity.LastModifiedOnUTC = DateTime.UtcNow;
            _apiDbContext.TestLicenseTypes.Update(existingEntity);
            if( await _apiDbContext.SaveChangesAsync() > 0)
            {
                return _mapper.Map<TestLicenseTypeSM>(existingEntity);
            }
            throw new CoreVisionException(ApiErrorTypeSM.NoRecord_Log, $"Test License Type for id: {model.Id} not found to update.", "Something went wrong, Please try again.");
        }

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes a license type from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the license type to delete.</param>
        /// <returns><c>true</c> if deletion was successful; otherwise, <c>false</c>.</returns>
        public async Task<BoolResponseRoot> DeleteLicenseType(int id)
        {
            var entity = await _apiDbContext.TestLicenseTypes.FindAsync(id);
            if (entity == null)
            {
                return new BoolResponseRoot(false, $"Test License Type for id: {id} not found to delete.");
            }

            _apiDbContext.TestLicenseTypes.Remove(entity);
            if(await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new BoolResponseRoot(true, $"Test License Type deleted successfully.");
            }
            throw new CoreVisionException(ApiErrorTypeSM.NoRecord_Log, $"Test License Type for id: {id} not found to delete.", "Something went wrong, Please try again.");
        }

        #endregion Delete
    }
}
