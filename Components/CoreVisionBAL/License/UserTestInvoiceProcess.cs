using CoreVisionServiceModels.Foundation.Base.Interfaces;
using AutoMapper;
using CoreVisionDAL.Context;
using CoreVisionBAL.Foundation.Base;
using CoreVisionServiceModels.v1.General.License;
using Stripe;
using CoreVisionDomainModels.v1.General.License;
using Microsoft.EntityFrameworkCore;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;

namespace CoreVisionBAL.License
{
    public class UserTestInvoiceProcess : CoreVisionBalOdataBase<UserTestInvoiceSM>
    {
        #region Properties
        private readonly ILoginUserDetail _loginUserDetail;
        #endregion Properties

        #region Constructor
        public UserTestInvoiceProcess(IMapper mapper, ApiDbContext apiDbContext, ILoginUserDetail loginUserDetail) : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }
        #endregion Constructor

        #region Odata

        /// <summary>
        /// This method gets any UserInvoice(s) by filtering/sorting the data
        /// </summary>
        /// <returns>UserInvoice(s)</returns>
        public override async Task<IQueryable<UserTestInvoiceSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.UserTestInvoices;
            IQueryable<UserTestInvoiceSM> retSM = await base.MapEntityAsToQuerable<UserTestInvoiceDM, UserTestInvoiceSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region --Count--

        /// <summary>
        /// Get UserInvoices Count in database.
        /// </summary>
        /// <returns>integer response</returns>

        public async Task<int> GetAllUserInvoicesCountResponse()
        {
            int resp = _apiDbContext.UserTestInvoices.AsNoTracking().Count();
            return resp;
        }

        #endregion --Count--

        #region Get All

        #region Get All
        /// <summary>
        /// Retrieves a list of all user invoices.
        /// </summary>
        /// <returns>A list of UserTestInvoiceSM or null if no invoices are found.</returns>
        public async Task<List<UserTestInvoiceSM>?> GetAllUserInvoices()
        {
            var userInvoicesFromDb = await _apiDbContext.UserTestInvoices.AsNoTracking().ToListAsync();
            if (userInvoicesFromDb == null)
            {
                return null;
            }
            return _mapper.Map<List<UserTestInvoiceSM>>(userInvoicesFromDb);
            
        }
        /// <summary>
        /// Retrieves a list of invoices for a specific user based on their user ID.
        /// This method queries the database for license details associated with the user
        /// that have a valid Stripe subscription ID. It then fetches the invoices related
        /// to those license details and returns them as a list.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom invoices are to be retrieved.</param>
        /// <returns>A task representing the asynchronous operation, with a list of user invoices as the result.</returns>
        /// <exception cref="CoreVisionException">Thrown when no invoices are found for the given user.</exception>
        public async Task<List<UserTestInvoiceSM>>GetMineInvoices(int userId)
        {
            var licenseDetailIds = await _apiDbContext.UserTestLicenseDetails.Where(x=>x.ClientUserId == userId && x.TestLicenseTypeId != 1).Select(x=>x.Id).ToListAsync();
            if(licenseDetailIds.Count == 0)
            {
                return new List<UserTestInvoiceSM>();
            }

            var response = new List<UserTestInvoiceSM>();
            foreach(var id in licenseDetailIds)
            {
                var res = await GetUserInvoiceByLicenseTypeId(id);
                if(res != null)
                {
                    response.Add(res);
                }
            }
            return response;
        }        

        #endregion Get All


        #endregion Get All

        #region Get Single

        #region Get By Id
        /// <summary>
        /// Retrieves a user invoice by its unique ID.
        /// </summary>
        /// <param name="Id">The ID of the user invoice to retrieve.</param>
        /// <returns>The UserTestInvoiceSM with the specified ID, or null if not found.</returns>
        public async Task<UserTestInvoiceSM?> GetUserInvoiceById(int Id)
        {
            var dm = await _apiDbContext.UserTestInvoices.FindAsync(Id);     
            if (dm == null)
            {
                return null;
            }
            return _mapper.Map<UserTestInvoiceSM>(dm);  
            
        }

        /// <summary>
        /// Retrieves a user invoice by its license type ID.
        /// </summary>
        /// <param name="Id">The ID of the user invoice to retrieve.</param>
        /// <returns>The UserTestInvoiceSM with the specified ID, or null if not found.</returns>
        public async Task<UserTestInvoiceSM?> GetUserInvoiceByLicenseTypeId(int userLicenseDetailId)
        {
            var singleUserInvoiceFromDb = await _apiDbContext.UserTestInvoices.Where(x=>x.UserTestLicenseDetailsId == userLicenseDetailId).FirstOrDefaultAsync();
            if (singleUserInvoiceFromDb == null)
            {
                return null;
            }
            return _mapper.Map<UserTestInvoiceSM>(singleUserInvoiceFromDb);

        }

        #endregion Get By Id

        #endregion Get Single

        #region Add
        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="UserTestInvoiceSM">The UserTestInvoiceSM to be added.</param>
        /// <returns>The added UserTestInvoiceSM, or null if addition fails.</returns>
        public async Task<UserTestInvoiceSM?> AddUserInvoice(UserTestInvoiceSM UserTestInvoiceSM)
        {
            if (UserTestInvoiceSM == null)
                return null;

            var UserTestInvoiceDM = _mapper.Map<UserTestInvoiceDM>(UserTestInvoiceSM);
            UserTestInvoiceDM.CreatedBy = _loginUserDetail.LoginId;
            UserTestInvoiceDM.CreatedOnUTC = DateTime.UtcNow;

            try
            {
                await _apiDbContext.UserTestInvoices.AddAsync(UserTestInvoiceDM);
                if (await _apiDbContext.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<UserTestInvoiceSM>(UserTestInvoiceDM);
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"Message: {ex.Message}, Exception: {ex.InnerException}", @"Could not add user invoice, please try again");
            }

            return null;
        }

        #endregion Add

        #region Update
        /// <summary>
        /// Updates a user invoice in the database.
        /// </summary>
        /// <param name="objIdToUpdate">The Id of the job opening to update.</param>
        /// <param name="UserTestInvoiceSM">The updated UserTestInvoiceSM object.</param>
        /// <returns>
        /// If successful, returns the updated UserTestInvoiceSM; otherwise, returns null.
        /// </returns>
        public async Task<UserTestInvoiceSM?> UpdateUserInvoice(int objIdToUpdate, UserTestInvoiceSM UserTestInvoiceSM)
        {
            try
            {
                if (UserTestInvoiceSM != null && objIdToUpdate > 0)
                {
                    //retrieves target user invoice from db
                    //UserTestInvoiceDM? objDM = await _apiDbContext.UserInvoices.FindAsync(objIdToUpdate);
                    UserTestInvoiceDM? objDM = await _apiDbContext.UserTestInvoices.Where(x=>x.Id == objIdToUpdate).FirstOrDefaultAsync();

                    if (objDM != null)
                    {
                        UserTestInvoiceSM.Id = objIdToUpdate;
                        _mapper.Map(UserTestInvoiceSM, objDM);

                        objDM.LastModifiedBy = _loginUserDetail.LoginId;
                        objDM.LastModifiedOnUTC = DateTime.UtcNow;

                        if (await _apiDbContext.SaveChangesAsync() > 0)
                        {
                            return _mapper.Map<UserTestInvoiceSM>(objDM);
                        }
                        return null;
                    }
                    else
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User invoice not found: {objIdToUpdate}", "User invoice to update not found, add as new instead.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"Message: {ex.Message}", @"Could not user invoice, please try again");
            }
            return null;
        }

        #endregion Update

        #region Delete
        /// <summary>
        /// Deletes a user invoice by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the user invoice to be deleted.</param>
        /// <returns>A DeleteResponseRoot indicating the result of the deletion operation.</returns>
        public async Task<DeleteResponseRoot> DeleteUserInvoiceById(int id)
        {
            try
            {
                // Check if the product with the specified ID exists in the database
                var isPresent = await _apiDbContext.UserInvoices.AnyAsync(x => x.Id == id);

                if (isPresent)
                {
                    // Create an instance of ProductDM with the specified ID for deletion
                    var dmToDelete = new UserTestInvoiceDM() { Id = id };

                    // Remove the user invoice from the database
                    _apiDbContext.UserTestInvoices.Remove(dmToDelete);

                    // Save changes to the database
                    if (await _apiDbContext.SaveChangesAsync() > 0)
                    {
                        // If deletion is successful, return a success response
                        return new DeleteResponseRoot(true, "User invoice with Id " + id + " deleted successfully!");
                    }
                }

                // If no product was found with the specified ID, return a failure response
                return new DeleteResponseRoot(false, "No such invoice found");
            }
            catch (Exception ex)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, @$"{ex.Message}", @"Could not delete user invoice, please try again", ex.InnerException);
            }
        }

        #endregion Delete

    }
}
