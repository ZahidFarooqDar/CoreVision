using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.EntityFrameworkCore;
using CoreVisionServiceModels.Foundation.Base.Enums;
using Stripe;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using AutoMapper;
using CoreVisionDAL.Context;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;

namespace CoreVisionBAL.License
{
    public class UserLicenseDetailsProcess : CoreVisionBalOdataBase<UserLicenseDetailsSM>
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        #endregion Properties

        #region Constructor
        public UserLicenseDetailsProcess(IMapper mapper, ApiDbContext apiDbContext,
            ILoginUserDetail loginUserDetail) : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }
        #endregion Constructor

        #region Odata
        /// <summary>
        /// This method gets any UserLicenseDetail(s) by filtering/sorting the data
        /// </summary>
        /// <returns>UserLicenseDetail(s)</returns>
        public override async Task<IQueryable<UserLicenseDetailsSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.UserLicenseDetails;
            IQueryable<UserLicenseDetailsSM> retSM = await base.MapEntityAsToQuerable<UserLicenseDetailsDM, UserLicenseDetailsSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region Get All
        /// <summary>
        /// Retrieves a list of all user subscriptions.
        /// </summary>
        /// <returns>A list of UserLicenseDetailsSM or null if no subscriptions are found.</returns>
        public async Task<List<UserLicenseDetailsSM>?> GetAllUserLicenseDetails()
        {
            try
            {
                var productsFromDb = await _apiDbContext.UserLicenseDetails.ToListAsync();
                if (productsFromDb == null)
                    return null;
                return _mapper.Map<List<UserLicenseDetailsSM>>(productsFromDb);
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not get products, please try again", ex.InnerException);
            }
        }

        #endregion Get All

        #region Get Single

        #region Get By UserId
        /// <summary>
        /// Retrieves a user subscription by its UserID.
        /// </summary>
        /// <param name="Id">The ID of the user subscription to retrieve.</param>
        /// <returns>The UserLicenseDetailsSM with the specified ID, or null if not found.</returns>
        public async Task<UserLicenseDetailsSM?> GetUserSubscriptionByUserId(int userId)
        {
            try
            {
                var singleUserSubscriptionFromDb = await _apiDbContext.UserLicenseDetails.Where(x => x.ClientUserId == userId).FirstOrDefaultAsync();
                if (singleUserSubscriptionFromDb == null)
                    return null;
                return _mapper.Map<UserLicenseDetailsSM>(singleUserSubscriptionFromDb);
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not get the product, please try again", ex.InnerException);
            }
        }
        #endregion Get By UserId

        #region Get By Id

        /// <summary>
        /// Retrieves a user subscription by its unique ID.
        /// </summary>
        /// <param name="Id">The ID of the user subscription to retrieve.</param>
        /// <returns>The UserLicenseDetailsSM with the specified ID, or null if not found.</returns>
        public async Task<UserLicenseDetailsSM?> GetUserSubscriptionById(int Id)
        {
            try
            {
                var singleUserSubscriptionFromDb = await _apiDbContext.UserLicenseDetails.FindAsync(Id);
                if (singleUserSubscriptionFromDb == null)
                    return null;
                return _mapper.Map<UserLicenseDetailsSM>(singleUserSubscriptionFromDb);
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not get the product, please try again", ex.InnerException);
            }
        }

        #endregion Get By Id

        #region Get By Stripe Customer ID and userId
        /// <summary>
        /// Retrieves a user subscription by its stripeCustomerId
        /// </summary>
        /// <param name="stripeCustomerId">The stripeCustomerID of the user subscription to retrieve.</param>
        /// <returns>The UserLicenseDetailsSM with the specified ID, or null if not found.</returns>
        public async Task<UserLicenseDetailsSM?> GetUserSubscriptionByStripeCustomerId(string stripeCustomerId, int userId)
        {
            try
            {
                var singleUserSubscriptionFromDb = await _apiDbContext.UserLicenseDetails.FirstOrDefaultAsync(x => x.StripeCustomerId == stripeCustomerId && x.ClientUserId == userId);
                if (singleUserSubscriptionFromDb == null)
                    return null;
                return _mapper.Map<UserLicenseDetailsSM>(singleUserSubscriptionFromDb);
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not get the product, please try again", ex.InnerException);
            }
        }
        #endregion Get By Stripe Customer ID


        #endregion Get Single

        #region Add
        /// <summary>
        /// Adds a new user subscription to the database.
        /// </summary>
        /// <param name="UserLicenseDetailsSM">The UserLicenseDetailsSM to be added.</param>
        /// <returns>The added UserLicenseDetailsSM, or null if addition fails.</returns>
        public async Task<UserLicenseDetailsSM?> AddUserSubscription(UserLicenseDetailsSM UserLicenseDetailsSM)
        {
            if (UserLicenseDetailsSM == null)
                return null;
           
            var UserLicenseDetailsDM = _mapper.Map<UserLicenseDetailsDM>(UserLicenseDetailsSM);
            UserLicenseDetailsDM.CreatedBy = _loginUserDetail.LoginId;
            UserLicenseDetailsDM.CreatedOnUTC = DateTime.UtcNow;

            try
            {
                await _apiDbContext.UserLicenseDetails.AddAsync(UserLicenseDetailsDM);
                if (await _apiDbContext.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<UserLicenseDetailsSM>(UserLicenseDetailsDM);
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not add user subscription, please try again", ex.InnerException);
            }

            return null;
        }


        public async Task<UserLicenseDetailsSM?> AddUserDummySubscription(int clientUserId, UserLicenseDetailsSM objSM)
        {
            if (objSM == null)
                return null;
            var existingLicenses = await _apiDbContext.UserLicenseDetails.Where(x => x.ClientUserId == clientUserId).ToListAsync();
            if(existingLicenses.Count > 0)
            {
                foreach(var lic in existingLicenses)
                {
                    lic.IsCancelled = true;
                    lic.IsSuspended = true;
                    lic.Status = "inactive";
                }
                await _apiDbContext.SaveChangesAsync();
            }
            var dm = _mapper.Map<UserLicenseDetailsDM>(objSM);
            dm.ProductName = "Data Raptor";
            dm.IsSuspended = false;
            dm.Status = "active";
            dm.Currency = "inr";
            dm.IsCancelled = false;

            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            try
            {
                await _apiDbContext.UserLicenseDetails.AddAsync(dm);
                if (await _apiDbContext.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<UserLicenseDetailsSM>(dm);
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not add user subscription, please try again", ex.InnerException);
            }

            return null;
        }

        #endregion Add


        #region Already Existing Plan

        public async Task<int> CheckAdditionalDays(int clientId, int newLicenseId)
        {
            var existingLicense = await _apiDbContext.UserLicenseDetails
                .Where(x => x.ClientUserId == clientId && x.Status == "active" && x.IsCancelled == false && x.IsSuspended == false)
                .FirstOrDefaultAsync();

            if (existingLicense == null)
                return 0;

            if (existingLicense.LicenseTypeId == newLicenseId) 
            {
                throw new CoreVisionException(ApiErrorTypeSM.Access_Denied_Log, "You are already subscribed to this plan.");
            }

            var daysRemaining = (existingLicense.ExpiryDateUTC.HasValue)
                ? (existingLicense.ExpiryDateUTC.Value - DateTime.UtcNow).TotalDays
                : 0;

            if (daysRemaining <= 0)
                return 0;

            var existingLicenseDetails = await _apiDbContext.LicenseTypes.FindAsync(existingLicense.LicenseTypeId);
            if (existingLicenseDetails == null)
                return 0;

            var moneyPerDayChargeExisting = existingLicenseDetails.Amount / existingLicenseDetails.ValidityInDays;
            var totalMoneyToRefund = moneyPerDayChargeExisting * daysRemaining;

            var newLicenseDetails = await _apiDbContext.LicenseTypes.FindAsync(newLicenseId);
            if (newLicenseDetails == null)
                return 0;

            var moneyPerDayChargeNew = newLicenseDetails.Amount / newLicenseDetails.ValidityInDays;
            var additionalDays = totalMoneyToRefund / moneyPerDayChargeNew;

            // Retrieve all existing licenses for the user
            var allExistingLicenses = await _apiDbContext.UserLicenseDetails
                .Where(x => x.ClientUserId == clientId && x.Status == "active" && x.IsCancelled == false && x.IsSuspended == false)
                .ToListAsync();

            // Update properties for each existing license
            foreach (var license in allExistingLicenses)
            {
                license.Status = "inactive";
                license.IsSuspended = true;
                license.IsCancelled = true;
                license.LastModifiedOnUTC = DateTime.UtcNow; // Optional: track the modification time
                license.LastModifiedBy = _loginUserDetail.LoginId; // Optional: track the user making changes
            }

            // Save the changes to the database
            _apiDbContext.UserLicenseDetails.UpdateRange(allExistingLicenses);
            await _apiDbContext.SaveChangesAsync();

            return (int)(newLicenseDetails.ValidityInDays + additionalDays);
        }


        #endregion Already Existing Plan

        #region Update
        /// <summary>
        /// Updates a user subscription in the database.
        /// </summary>
        /// <param name="objIdToUpdate">The Id of the job opening to update.</param>
        /// <param name="UserLicenseDetailsSM">The updated UserLicenseDetailsSM object.</param>
        /// <returns>
        /// If successful, returns the updated UserLicenseDetailsSM; otherwise, returns null.
        /// </returns>
        public async Task<UserLicenseDetailsSM?> UpdateUserSubscription(int objIdToUpdate, UserLicenseDetailsSM UserLicenseDetailsSM)
        {
            try
            {
                if (UserLicenseDetailsSM != null && objIdToUpdate > 0)
                {
                    //retrieves target user subscription from db
                    UserLicenseDetailsDM? objDM = await _apiDbContext.UserLicenseDetails.FindAsync(objIdToUpdate);

                    if (objDM != null)
                    {
                        UserLicenseDetailsSM.Id = objIdToUpdate;
                        _mapper.Map(UserLicenseDetailsSM, objDM);

                        objDM.LastModifiedBy = _loginUserDetail.LoginId;
                        objDM.LastModifiedOnUTC = DateTime.UtcNow;

                        if (await _apiDbContext.SaveChangesAsync() > 0)
                        {
                            return _mapper.Map<UserLicenseDetailsSM>(objDM);
                        }
                        return null;
                    }
                    else
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User subscription not found: {objIdToUpdate}", "User subscription to update not found, add as new instead.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not update user subscription, please try again", ex.InnerException);
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
                // Check if the product with the specified ID exists in the database
                var isPresent = await _apiDbContext.UserLicenseDetails.AnyAsync(x => x.Id == id);

                if (isPresent)
                {
                    // Create an instance of UserLicenseDetailsDM with the specified ID for deletion
                    var dmToDelete = new UserLicenseDetailsDM() { Id = id };

                    // Remove the user subscription from the database
                    _apiDbContext.UserLicenseDetails.Remove(dmToDelete);

                    // Save changes to the database
                    if (await _apiDbContext.SaveChangesAsync() > 0)
                    {
                        // If deletion is successful, return a success response
                        return new DeleteResponseRoot(true, "User subscription with Id " + id + " deleted successfully!");
                    }
                }

                // If no product was found with the specified ID, return a failure response
                return new DeleteResponseRoot(false, "No such user subscription found");
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not delete the user subscription, please try again", ex.InnerException);
            }
        }

        #endregion Delete

        #region Mine-License

        public async Task<UserLicenseDetailsSM> GetActiveTrialUserLicenseDetailsByUserId(int currentUserId)
        {
            
            DateTime startDateTime = DateTime.UtcNow;
            var validityInDays = 15;
            DateTime endDateTime = DateTime.UtcNow.AddDays(validityInDays);

            var existingTrialLicense = await _apiDbContext.UserLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == currentUserId && x.SubscriptionPlanName == "Trial" && x.Status == "active");
            if (existingTrialLicense != null)
            {
                // Check if the trial license has expired
                if (existingTrialLicense.ExpiryDateUTC.Value.Date < DateTime.UtcNow.Date)
                {
                    await UpdateLicenseStatus(existingTrialLicense.Id);
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial Period has been ended. Buy a new license to continue", "Trial Period has been ended. Buy a new license to continue");
                }
                // Trial license already exists for the user
                //throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial license already exists for user", "Trial license already exists for user");
                return  _mapper.Map<UserLicenseDetailsSM>(existingTrialLicense);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Retrieves the active license details for a given user. 
        /// If the license has expired, an exception is thrown with a message prompting the user to purchase a new license. 
        /// If no active license is found, an exception is thrown indicating that no active license exists and the user needs to activate one.
        /// </summary>
        /// <param name="currentUserId">The ID of the user whose active license details are to be retrieved.</param>
        /// <returns>
        /// Returns the active license details mapped to a UserLicenseDetailsSM object if an active license exists and is valid.
        /// </returns>
        /// <exception cref="CoreVisionException">
        /// Thrown when the user's license has expired, or no active license exists.
        /// </exception>
        /*public async Task<UserLicenseDetailsSM> GetActiveLicenseDetailsByUserId(int currentUserId)
        {

            DateTime startDateTime = DateTime.UtcNow;

            var existingLicenseDetails = await _apiDbContext.UserLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == currentUserId && x.Status == "active");
            if (existingLicenseDetails != null)
            {
                if (existingLicenseDetails.ExpiryDateUTC.Value.Date < DateTime.UtcNow.Date)
                {
                    
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "License Period has been ended. Buy a new license to continue", "License Period has been ended. Buy a new license to continue");
                }
                var licenseType = await _apiDbContext.LicenseTypes.Where(x=>x.StripePriceId == existingLicenseDetails.StripePriceId).AsNoTracking().FirstOrDefaultAsync();
                if(existingLicenseDetails.LicenseTypeId != licenseType.Id)
                {
                    existingLicenseDetails.LicenseTypeId = licenseType.Id;
                    await _apiDbContext.SaveChangesAsync();
                }
                var response = _mapper.Map<UserLicenseDetailsSM>(existingLicenseDetails);
                return response;
            }
            else
            {
                *//*var response = new UserLicenseDetailsSM()
                {

                }*//*
                return new UserLicenseDetailsSM();
               *//*// return null;
                throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                    "No active license found for the user. Please activate a license to proceed.",
                    "No active license found for the user. Please activate a license to proceed.");*//*
            }
        }*/

        public async Task<UserLicenseDetailsSM> GetActiveLicenseDetailsByUserId(int currentUserId)
        {

            DateTime startDateTime = DateTime.UtcNow;
            var licenseDetail = new UserLicenseDetailsSM();

            var existingLicenseDetails = await _apiDbContext.UserLicenseDetails.FirstOrDefaultAsync(x => x.ClientUserId == currentUserId && x.Status == "active");
            if (existingLicenseDetails != null)
            {
                if (existingLicenseDetails.ExpiryDateUTC.Value.Date < DateTime.UtcNow.Date)
                {
                    licenseDetail = _mapper.Map<UserLicenseDetailsSM>(existingLicenseDetails);
                    licenseDetail.Status = "expired";
                    return licenseDetail;
                    //throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "License Period has been ended. Buy a new license to continue", "License Period has been ended. Buy a new license to continue");
                }
                var licenseType = await _apiDbContext.LicenseTypes.Where(x => x.StripePriceId == existingLicenseDetails.StripePriceId).AsNoTracking().FirstOrDefaultAsync();
                if (existingLicenseDetails.LicenseTypeId != licenseType.Id)
                {
                    existingLicenseDetails.LicenseTypeId = licenseType.Id;
                    await _apiDbContext.SaveChangesAsync();
                }
                licenseDetail = _mapper.Map<UserLicenseDetailsSM>(existingLicenseDetails);
                return licenseDetail;
            }
            else
            {
                licenseDetail.Status = "expired";
                licenseDetail.ExpiryDateUTC = DateTime.UtcNow;
                return licenseDetail;
                //return new UserLicenseDetailsSM();
                /*throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                    "No active license found for the user. Please activate a license to proceed.",
                    "No active license found for the user. Please activate a license to proceed.");*/
            }
        }

        #endregion Mine-License

        #region Trial License Methods

        /// <summary>
        /// Adds a trial license for a user, ensuring that no active or expired trial license already exists.
        /// </summary>
        /// <param name="userId">The ID of the user to add the trial license for.</param>
        /// <returns>A <see cref="UserLicenseDetailsSM"/> object with the details of the added trial license, or null if not successful.</returns>
        /// <exception cref="CoreVisionException">
        /// Thrown if an active trial license exists, the trial period has ended, or an expired trial license already exists.
        /// </exception>
        public async Task<UserLicenseDetailsSM?> AddTrialLicenseDetails(int userId)
        {
            DateTime startDateTime = DateTime.UtcNow;

            // Check for existing active trial license
            var existingTrialLicense = await _apiDbContext.UserLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.SubscriptionPlanName == "Trial");

            if (existingTrialLicense != null)
            {
                if (existingTrialLicense.Status == "active" && existingTrialLicense.ExpiryDateUTC > startDateTime)
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial license already exists for user", "Trial license already exists for user");
                }
                else if (existingTrialLicense.Status == "active")
                {
                    await UpdateLicenseStatus(existingTrialLicense.Id);
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Your Trial Period is Over", "Your Trial Period is Over");
                }
                else if (existingTrialLicense.Status == "trial_period_ended")
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial Period has ended. Purchase a new license to continue", "Trial Period has ended. Purchase a new license to continue");
                }
            }
            //var customerId = await CreateStripeCustomer(userId);
            // Add new trial license
            var userLicenseDetailDM = new UserLicenseDetailsDM
            {
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = startDateTime,
                ValidityInDays = 15,
                LicenseTypeId = 1,
                ProductName = "Data Raptor Trial License",
                SubscriptionPlanName = "Trial",
                ExpiryDateUTC = startDateTime.AddDays(15),
                CancelAt = startDateTime.AddDays(15),
                StartDateUTC = startDateTime,
                IsCancelled = false,
                IsSuspended = false,
                CancelledOn = null,
                ClientUserId = userId,
                StripeSubscriptionId = "data_raptor_trial",
                ActualPaidPrice = 0,
                Currency = "nill",
                Status = "active",
                DiscountInPercentage = 0,
                UserInvoices = null,
                //StripeCustomerId = customerId,
                StripePriceId = "0000"
            };

            await _apiDbContext.UserLicenseDetails.AddAsync(userLicenseDetailDM);
            return await _apiDbContext.SaveChangesAsync() > 0
                ? _mapper.Map<UserLicenseDetailsSM>(userLicenseDetailDM)
                : null;
        }
        public async Task<UserLicenseDetailsSM?> AddOrUpdateTrialLicenseDetails(int userId)
        {
            DateTime currentUtcTime = DateTime.UtcNow;

            //Todo: Check whether to implement this logic for users who already have or had license other than trial
            /*var userWithLicense = await _apiDbContext.UserLicenseDetails
                .Where(x => x.ClientUserId == userId && x.SubscriptionPlanName != "Trial")
                .FirstOrDefaultAsync();

            if (userWithLicense != null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    "Trial period is not available for this user.",
                    "You are not eligible for a trial period.");
            }*/
            // Check if an active trial license already exists
            var existingActiveTrialLicense = await _apiDbContext.UserLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.SubscriptionPlanName == "Trial" && x.Status == "active");

            if (existingActiveTrialLicense != null)
            {
                // If an active trial exists but is expired, update status and throw an exception
                if (existingActiveTrialLicense.ExpiryDateUTC <= currentUtcTime)
                {
                    await UpdateLicenseStatus(existingActiveTrialLicense.Id);
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                        "Your Trial Period is Over",
                        "Your Trial Period is Over");
                }
                else
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                        "Trial license already exists for user",
                        "Trial license already exists for user");
                }
            }

            // Check if the user has a trial license that has ended
            var endedTrialLicense = await _apiDbContext.UserLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.Status == "trial_period_ended");

            if (endedTrialLicense != null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                    "Trial Period has ended. Please purchase a new license to continue.",
                    "Trial Period has ended. Please purchase a new license to continue.");
            }

            // Check if the user has any existing trial license (active or not)
            var existingTrialLicense = await _apiDbContext.UserLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.SubscriptionPlanName == "Trial");

            if (existingTrialLicense != null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog,
                    "Trial license already exists for the user",
                    "Trial license already exists for the user");
            }

            // Create and save a new trial license
            var newTrialLicense = new UserLicenseDetailsDM
            {
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = currentUtcTime,
                ValidityInDays = 15,  // 15 days validity for trial license
                LicenseTypeId = 1,
                ProductName = "Data Raptor Trial License",
                SubscriptionPlanName = "Trial",
                ExpiryDateUTC = currentUtcTime.AddDays(15),  // 7 days trial period
                CancelAt = currentUtcTime.AddDays(15),
                StartDateUTC = currentUtcTime,
                IsCancelled = false,
                IsSuspended = false,
                CancelledOn = null,
                ClientUserId = userId,
                StripeSubscriptionId = "data_raptor_trial",
                ActualPaidPrice = 0,
                Currency = "nill",
                Status = "active",
                DiscountInPercentage = 0,
                UserInvoices = null,
                StripePriceId = "0000"
            };

            await _apiDbContext.UserLicenseDetails.AddAsync(newTrialLicense);

            // Save the new trial license and return the mapped service model
            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return _mapper.Map<UserLicenseDetailsSM>(newTrialLicense);
            }

            return null;
        }

        public async Task<UserLicenseDetailsSM?> UpdateLicenseStatus(int id)
        {
            var userLicenseDetails = await _apiDbContext.UserLicenseDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (userLicenseDetails != null)
            {
                userLicenseDetails.IsCancelled = true;
                userLicenseDetails.IsSuspended = true;
                userLicenseDetails.CancelledOn = DateTime.UtcNow;
                userLicenseDetails.ExpiryDateUTC = DateTime.UtcNow;
                userLicenseDetails.Status = "trial_period_ended";
                _apiDbContext.UserLicenseDetails.Update(userLicenseDetails);
                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return _mapper.Map<UserLicenseDetailsSM>(userLicenseDetails);
            }
            return null;
        }
        #endregion Trial License Methods

        #region Additional

        #region Add New Stripe Customer and Add Trial license as well

        public async Task<string> CreateStripeCustomer(int userId)
        {
            var user = await _apiDbContext.ClientUsers.Where(x => x.Id == userId).FirstOrDefaultAsync();

            // Create options for creating a new customer
            var options = new CustomerCreateOptions
            {
                Name = user.LoginId,
                Email = user.EmailId
            };

            // Initialize the CustomerService
            var service = new Stripe.CustomerService();

            // Call the CreateAsync method with the options to create a new customer
            var customer = await service.CreateAsync(options);
            /*if(customer != null)
            {
                var userDetails = await _userLicenseDetailsProcess.GetUserSubscriptionByUserId(userId);

                userDetails.StripeCustomerId = customer.Id;
                await _apiDbContext.SaveChangesAsync();
            }*/
            return customer.Id;

        }

        public async Task<UserLicenseDetailsSM?> AddTrialLicenseWithStripeCustomerDetails(int userId, string stripeCustomerId)
        {
            DateTime startDateTime = DateTime.UtcNow;

            // Check for existing active trial license
            var existingTrialLicense = await _apiDbContext.UserLicenseDetails
                .FirstOrDefaultAsync(x => x.ClientUserId == userId && x.SubscriptionPlanName == "Trial");

            if (existingTrialLicense != null)
            {
                if (existingTrialLicense.Status == "active" && existingTrialLicense.ExpiryDateUTC > startDateTime)
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial license already exists for user", "Trial license already exists for user");
                }
                else if (existingTrialLicense.Status == "active")
                {
                    await UpdateLicenseStatus(existingTrialLicense.Id);
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Your Trial Period is Over", "Your Trial Period is Over");
                }
                else if (existingTrialLicense.Status == "trial_period_ended")
                {
                    throw new CoreVisionException(ApiErrorTypeSM.NoRecord_NoLog, "Trial Period has ended. Purchase a new license to continue", "Trial Period has ended. Purchase a new license to continue");
                }
            }

            // Add new trial license
            var userLicenseDetailDM = new UserLicenseDetailsDM
            {
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = startDateTime,
                ValidityInDays = 15,
                LicenseTypeId = 1,
                ProductName = "Data Raptor",
                SubscriptionPlanName = "Trial",
                ExpiryDateUTC = startDateTime.AddDays(15),
                CancelAt = startDateTime.AddDays(15),
                StartDateUTC = startDateTime,
                IsCancelled = false,
                IsSuspended = false,
                CancelledOn = null,
                ClientUserId = userId,
                StripeSubscriptionId = "data_raptor_trial",
                ActualPaidPrice = 0,
                Currency = "nill",
                Status = "active",
                DiscountInPercentage = 0,
                UserInvoices = null,
                StripeCustomerId = stripeCustomerId,
                StripePriceId = "0000"
            };

            await _apiDbContext.UserLicenseDetails.AddAsync(userLicenseDetailDM);
            return await _apiDbContext.SaveChangesAsync() > 0
                ? _mapper.Map<UserLicenseDetailsSM>(userLicenseDetailDM)
                : null;
        }
        #endregion Add New Stripe Customer and Add Trial license as well

        #endregion Additional
    }
}
