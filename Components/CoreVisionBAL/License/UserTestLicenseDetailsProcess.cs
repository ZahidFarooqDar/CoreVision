using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionBAL.License
{
    public class UserTestLicenseDetailsProcess : CoreVisionBalOdataBase<UserTestLicenseDetailsSM>
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        private readonly APIConfiguration _apiConfiguration;
        #endregion Properties

        #region Constructor
        public UserTestLicenseDetailsProcess(IMapper mapper, ApiDbContext apiDbContext, APIConfiguration apiConfiguration,
            ILoginUserDetail loginUserDetail) : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
            _apiConfiguration = apiConfiguration;
        }
        #endregion Constructor

        #region Odata
        /// <summary>
        /// This method gets any TestUserLicenseDetail(s) by filtering/sorting the data
        /// </summary>
        /// <returns>TestUserLicenseDetail(s)</returns>
        public override async Task<IQueryable<UserTestLicenseDetailsSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.UserTestLicenseDetails;
            IQueryable<UserTestLicenseDetailsSM> retSM = await base.MapEntityAsToQuerable<UserTestLicenseDetailsDM, UserTestLicenseDetailsSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region Count

        public async Task<int> GetAllTestUserLicenseDetailsCount()
        {
            var count = await _apiDbContext.UserTestLicenseDetails.AsNoTracking().CountAsync();

            return count;
        }

        #endregion Count

        #region Get All
        /// <summary>
        /// Retrieves a list of all user subscriptions.
        /// </summary>
        /// <returns>A list of UserTestLicenseDetailsSM or null if no subscriptions are found.</returns>
        public async Task<List<UserTestLicenseDetailsSM>?> GetAllTestUserLicenseDetails()
        {
            var dms = await _apiDbContext.UserTestLicenseDetails.AsNoTracking().ToListAsync();
            if (dms.Count == 0)
            {
                return new List<UserTestLicenseDetailsSM>();
            }
            var list = _mapper.Map<List<UserTestLicenseDetailsSM>>(dms);

            return list;
        }

        #endregion Get All

        #region Get Single

        #region Get By UserId
        /// <summary>
        /// Retrieves a test user subscription by its UserId.
        /// </summary>
        /// <param name="Id">The ID of the test user subscription to retrieve.</param>
        /// <returns>The UserTestLicenseDetailsSM with the specified ID, or null if not found.</returns>
        public async Task<List<UserTestLicenseDetailsSM>> GetTestUserSubscriptionByUserId(int userId)
        {
            var dm = await _apiDbContext.UserTestLicenseDetails.AsNoTracking().Where(x => x.ClientUserId == userId).ToListAsync();
            if (dm.Count == 0)
            {
                return new List<UserTestLicenseDetailsSM>();
            }
            return _mapper.Map<List<UserTestLicenseDetailsSM>>(dm);

        }

        #endregion Get By UserId

        #region Get By Id

        /// <summary>
        /// Retrieves a test user subscription by its unique ID.
        /// </summary>
        /// <param name="Id">The ID of the user subscription to retrieve.</param>
        /// <returns>The UserTestLicenseDetailsSM with the specified ID, or null if not found.</returns>
        public async Task<UserTestLicenseDetailsSM?> GetUserSubscriptionById(int Id)
        {
            var singleUserSubscriptionFromDb = await _apiDbContext.UserTestLicenseDetails.FindAsync(Id);
            if (singleUserSubscriptionFromDb == null)
            {
                return null;
            }
            return _mapper.Map<UserTestLicenseDetailsSM>(singleUserSubscriptionFromDb);

        }


        #endregion Get By Id

        #endregion Get Single

        #region Add
        /// <summary>
        /// Adds a new user subscription to the database.
        /// </summary>
        /// <param name="UserTestLicenseDetailsSM">The UserTestLicenseDetailsSM to be added.</param>
        /// <returns>The added UserTestLicenseDetailsSM, or null if addition fails.</returns>
        public async Task<UserTestLicenseDetailsSM?> AddTestUserSubscription(UserTestLicenseDetailsSM UserTestLicenseDetailsSM, int testLicenseId, string userName)
        {
            if (UserTestLicenseDetailsSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @"UserTestLicenseDetailsSM is null", @"Please provide details to add test user subscription");
            }

            var existingUser = await _apiDbContext.ClientUsers
                .Where(x => x.LoginId == userName || x.EmailId == userName)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User is not present for UserName or Email {userName}", @"Please provide valid user to add test user subscription");
            }

            var existingLicense = await _apiDbContext.TestLicenseTypes
                .Where(x => x.Id == testLicenseId)
                .FirstOrDefaultAsync();

            if (existingLicense == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @"LicenseType is null", @"Please provide valid license type to add test user subscription");
            }

            var dm = _mapper.Map<UserTestLicenseDetailsDM>(UserTestLicenseDetailsSM);
            dm.ClientUserId = existingUser.Id;
            dm.TestLicenseTypeId = testLicenseId;
            dm.TestCountValidity = existingLicense.TestCountValidity;
            dm.LicenseStatus = LicenseStatusDM.Active;
            dm.IsSuspended = false;
            dm.IsCancelled = false;
            dm.StartDateUTC = DateTime.UtcNow;
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await using var transaction = await _apiDbContext.Database.BeginTransactionAsync();

            try
            {
                await _apiDbContext.UserTestLicenseDetails.AddAsync(dm);
                await _apiDbContext.SaveChangesAsync();

                var invoiceDetails = new UserTestInvoiceDM
                {
                    UserTestLicenseDetailsId = dm.Id,
                    DiscountInPercentage = dm.DiscountInPercentage,
                    ActualPaidPrice = dm.ActualPaidPrice,
                    RemainingAmount = dm.RemainingAmount,
                    CreatedBy = dm.CreatedBy,
                    CreatedOnUTC = dm.CreatedOnUTC
                };

                await _apiDbContext.UserTestInvoices.AddAsync(invoiceDetails);
                await _apiDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return _mapper.Map<UserTestLicenseDetailsSM>(dm);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"Message: {ex.Message}, Exception: {ex.InnerException}", @"Something went wrong, please try again");
            }
        }

        #endregion Add

        #region Update
        /// <summary>
        /// Updates a user subscription in the database.
        /// </summary>
        /// <param name="objIdToUpdate">The Id of the job opening to update.</param>
        /// <param name="UserTestLicenseDetailsSM">The updated UserTestLicenseDetailsSM object.</param>
        /// <returns>
        /// If successful, returns the updated UserTestLicenseDetailsSM; otherwise, returns null.
        /// </returns>
        public async Task<UserTestLicenseDetailsSM?> UpdateUserSubscription(int objIdToUpdate, UserTestLicenseDetailsSM UserTestLicenseDetailsSM)
        {
            try
            {
                if (UserTestLicenseDetailsSM != null && objIdToUpdate > 0)
                {
                    //retrieves target user subscription from db
                    UserTestLicenseDetailsDM? objDM = await _apiDbContext.UserTestLicenseDetails.FindAsync(objIdToUpdate);

                    if (objDM != null)
                    {
                        UserTestLicenseDetailsSM.Id = objIdToUpdate;
                        _mapper.Map(UserTestLicenseDetailsSM, objDM);

                        objDM.LastModifiedBy = _loginUserDetail.LoginId;
                        objDM.LastModifiedOnUTC = DateTime.UtcNow;

                        if (await _apiDbContext.SaveChangesAsync() > 0)
                        {
                            return _mapper.Map<UserTestLicenseDetailsSM>(objDM);
                        }
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User test subscription was not updated", "Something went ");
                    }
                    else
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User subscription not found: {objIdToUpdate}", "User subscription to update not found, add as new instead.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"Message: {ex.Message} and Exception: {ex.InnerException}", @"Could not update user subscription, please try again");
            }
            return null;
        }

        #endregion Update

        #region Delete
        /// <summary>
        /// Deletes a user subscription by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        /// <returns>A DeleteResponseRoot indicating the result of the deletion operation.</returns>
        public async Task<DeleteResponseRoot> DeleteUserSubscriptionById(int id)
        {
            try
            {
                var isPresent = await _apiDbContext.UserTestLicenseDetails.AnyAsync(x => x.Id == id);

                if (isPresent)
                {
                    // Create an instance of UserTestLicenseDetailsDM with the specified ID for deletion
                    var dmToDelete = new UserTestLicenseDetailsDM() { Id = id };

                    // Remove the user subscription from the database
                    _apiDbContext.UserTestLicenseDetails.Remove(dmToDelete);

                    // Save changes to the database
                    if (await _apiDbContext.SaveChangesAsync() > 0)
                    {
                        return new DeleteResponseRoot(true, "User subscription with Id " + id + " deleted successfully!");
                    }
                }

                // If no product was found with the specified ID, return a failure response
                return new DeleteResponseRoot(false, "No such user subscription found");
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"Message: {ex.Message}, Exception: {ex.InnerException}", @"Could not delete the user subscription, please try again");
            }
        }

        #endregion Delete

        #region Mine-License              

        public async Task<UserTestLicenseDetailsSM> GetActiveUserLicenseDetailsByUserId(int currentUserId)
        {
            UserTestLicenseDetailsDM? existingLicense = await _apiDbContext.UserTestLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == currentUserId && x.LicenseStatus == LicenseStatusDM.Active);
            if (existingLicense != null)
            {
                // Check if the trial license has expired
                if (existingLicense.LicenseStatus == LicenseStatusDM.Expired || existingLicense.IsCancelled == true || existingLicense.IsSuspended == true)
                {
                    throw new CoreVisionException(ApiErrorTypeSM.Access_Denied_Log,
                        $"Access denied for user ID: {currentUserId}.",
                        "Your license has expired. Please purchase a new license to continue using the service.");

                }
                return _mapper.Map<UserTestLicenseDetailsSM>(existingLicense);
            }
            else
            {
                throw new CoreVisionException(ApiErrorTypeSM.Access_Denied_Log,
                    $"Access denied for user ID: {currentUserId}.",
                    "No valid license found. Please purchase a new license to continue.");
            }
        }

        public async Task<int> GetActiveUserLicenseTestCountsByUserId(int currentUserId)
        {
            UserTestLicenseDetailsDM? existingLicense = await _apiDbContext.UserTestLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == currentUserId && x.LicenseStatus == LicenseStatusDM.Active);
            if (existingLicense != null)
            {
                // Check if the trial license has expired
                if (existingLicense.LicenseStatus == LicenseStatusDM.Expired || existingLicense.IsCancelled == true || existingLicense.IsSuspended == true)
                {
                    return 0;
                }
                return existingLicense.TestCountValidity;
            }
            else
            {
                return 0;
            }
        }


        #endregion Mine-License

        #region License Methods/Trial

        /// <summary>
        /// Adds a trial license for a user, ensuring that no active or expired trial license already exists.
        /// </summary>
        /// <param name="userId">The ID of the user to add the trial license for.</param>
        /// <returns>A <see cref="UserTestLicenseDetailsSM"/> object with the details of the added trial license, or null if not successful.</returns>
        /// <exception cref="CoreVisionException">
        /// Thrown if an active trial license exists, the trial period has ended, or an expired trial license already exists.
        /// </exception>
        public async Task<UserTestLicenseDetailsSM?> AddTrialLicenseDetails(int userId)
        {

            UserTestLicenseDetailsDM? existingTrialLicense = await _apiDbContext.UserTestLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.SubscriptionPlanName == "Trial");

            if (existingTrialLicense != null)
            {
                if (existingTrialLicense.LicenseStatus == LicenseStatusDM.Active && existingTrialLicense.IsCancelled == false && existingTrialLicense.IsSuspended == false)
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial license already exists for user", "Trial license already existed, enjoy your trial period.");
                }                
            }
            var testCountValidity = _apiConfiguration.TrialLicenseTestCount;
            var testUserLicenseDetailDM = new UserTestLicenseDetailsDM
            {
                CreatedBy = _loginUserDetail.LoginId,
                TestCountValidity = testCountValidity,
                TestLicenseTypeId = 1,
                SubscriptionPlanName = "Trial",
                StartDateUTC = DateTime.UtcNow,
                IsCancelled = false,
                IsSuspended = false,
                ClientUserId = userId,
                ActualPaidPrice = 0,
                LicenseStatus = LicenseStatusDM.Active,
                PaymentMethod = PaymentMethodDM.Other,
                DiscountInPercentage = 0,
            };

            await _apiDbContext.UserTestLicenseDetails.AddAsync(testUserLicenseDetailDM);
            return await _apiDbContext.SaveChangesAsync() > 0
                ? _mapper.Map<UserTestLicenseDetailsSM>(testUserLicenseDetailDM)
                : null;
        }        
        public async Task<UserTestLicenseDetailsSM?> UpdateLicenseStatusOfUser(int userId)
        {
            var dm = await _apiDbContext.UserTestLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == userId);
            if (dm != null)
            {
                if(dm.LicenseStatus == LicenseStatusDM.Expired || dm.IsCancelled == true || dm.IsSuspended == true)
                {
                    return _mapper.Map<UserTestLicenseDetailsSM>(dm);
                }
                dm.IsCancelled = true;
                dm.IsSuspended = true;
                dm.LicenseStatus = LicenseStatusDM.Expired;
                 _apiDbContext.UserTestLicenseDetails.Update(dm);
                if (await _apiDbContext.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<UserTestLicenseDetailsSM>(dm);
                }
            }
            throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, $"User not found for UserId: {userId}", "User not found.");
        }
        #endregion Trial License Methods

        #region Additional

        #endregion Additional
    }
}
