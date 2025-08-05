using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.v1.Examination;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.Examination;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionBAL.General.Examination
{
    public class ExamProcess : CoreVisionBalOdataBase<ExamSM>
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor
        public ExamProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata

        /// <summary>
        /// Retrieves a queryable collection of gift card service models for OData processing.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{ExamSM}"/>of type <see cref="ExamSM"/> representing the collection of gift cards.
        /// </returns>

        public override async Task<IQueryable<ExamSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Exams;
            IQueryable<ExamSM> retSM = await base.MapEntityAsToQuerable<ExamDM, ExamSM>(_mapper, entitySet);
            return retSM;
        }
        #endregion Odata

        #region --Count--

        /// <summary>
        /// Retrieves the total count of all exams.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> representing the total number of exams.
        /// </returns>
        public async Task<int> GetAllExamsCountResponse()
        {
            return _apiDbContext.Exams.AsNoTracking().Count();
        }

        /// <summary>
        /// Retrieves the total count of all active exams.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> representing the total number of exams.
        /// </returns>
        public async Task<int> GetAllActiveExamsCountResponse()
        {
            return _apiDbContext.Exams.AsNoTracking().Where(x=>x.IsActive==true).Count();
        }

        #endregion --Count--

        #region Get All

        /// <summary>
        /// Retrieves a list of all exams.
        /// </summary>
        /// <returns>
        /// A <see cref="List{ExamSM}"/> of all exams, or null if none exist.
        /// </returns>
        public async Task<List<ExamSM>?> GetAllExams()
        {
            var list = await _apiDbContext.Exams.AsNoTracking().ToListAsync();
            if (list == null)
                return null;

            return _mapper.Map<List<ExamSM>>(list);
        }

        /// <summary>
        /// Retrieves a list of all active exams.
        /// </summary>
        /// <returns>
        /// A <see cref="List{ExamSM}"/> of all exams, or null if none exist.
        /// </returns>
        public async Task<List<ExamSM>?> GetAllActiveExams()
        {
            var list = await _apiDbContext.Exams.AsNoTracking()
                .Where(x => x.IsActive == true)
                .ToListAsync();
            if (list == null)
                return null;

            return _mapper.Map<List<ExamSM>>(list);
        }

        #endregion Get All

        #region Get Single

        /// <summary>
        /// Retrieves a specific exam by its ID.
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <returns>The <see cref="ExamSM"/> object or null if not found.</returns>
        public async Task<ExamSM?> GetExamById(int id)
        {
            var dm = await _apiDbContext.Exams.FindAsync(id);
            if (dm == null)
                return null;

            return _mapper.Map<ExamSM>(dm);
        }

        #endregion Get Single

        #region Add

        /// <summary>
        /// Adds a new exam to the database.
        /// </summary>
        /// <param name="objSM">The exam to add.</param>
        /// <returns>The added <see cref="ExamSM"/>, or null if input is invalid.</returns>
        /// <exception cref="CoreVisionException">Thrown if save fails.</exception>
        public async Task<ExamSM?> AddExam(ExamSM objSM)
        {
            if (objSM == null)
                return null;

            var dm = _mapper.Map<ExamDM>(objSM);
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Exams.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetExamById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                $"Failed to add exam. See inner exception.",
                "Something went wrong while adding new Exam");
        }

        public async Task<BoolResponseRoot> AddListOfExams(List<ExamSM> objSM)
        {
            if (objSM.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,"Failed to add exams, Data not found to add", "Something went wrong while adding new Exam Types");
            }                

            foreach(var item in objSM)
            {
                await AddExam(item);
            }
            return new BoolResponseRoot(true , "Exams added successfully.");

        }       

        #endregion Add

        #region Update

        /// <summary>
        /// Updates an existing exam.
        /// </summary>
        /// <param name="objIdToUpdate">Exam ID to update.</param>
        /// <param name="objSM">Updated data.</param>
        /// <returns>The updated <see cref="ExamSM"/>, or null.</returns>
        public async Task<ExamSM?> UpdateExam(int objIdToUpdate, ExamSM objSM)
        {
            if (objSM != null && objIdToUpdate > 0)
            {
                var objDM = await _apiDbContext.Exams.FindAsync(objIdToUpdate);
                if (objDM != null)
                {
                    objSM.Id = objIdToUpdate;
                    _mapper.Map(objSM, objDM);
                    objDM.LastModifiedBy = _loginUserDetail.LoginId;
                    objDM.LastModifiedOnUTC = DateTime.UtcNow;

                    if (await _apiDbContext.SaveChangesAsync() > 0)
                        return await GetExamById(objIdToUpdate);

                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Error updating Exam ID:{objIdToUpdate}", "Update failed.");
                }
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exam ID:{objIdToUpdate} not found.", "Exam not found.");
            }

            return null;
        }

        /// <summary>
        /// Updates the status of a specific exam.
        /// </summary>
        /// <param name="objIdToUpdate">The exam ID.</param>
        /// <param name="status">New status.</param>
        /// <returns>The updated <see cref="ExamSM"/> or null if not found.</returns>
        public async Task<ExamSM?> UpdateExamStatus(int objIdToUpdate, bool status)
        {
            var objDM = await _apiDbContext.Exams.FindAsync(objIdToUpdate);
            if (objDM != null)
            {
                objDM.IsActive = status;
                objDM.LastModifiedBy = _loginUserDetail.LoginId;
                objDM.LastModifiedOnUTC = DateTime.UtcNow;

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return await GetExamById(objIdToUpdate);

                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Failed to update status for Exam ID:{objIdToUpdate}", "Status update failed.");
            }

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exam ID:{objIdToUpdate} not found.", "Exam not found.");
        }

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes an exam by ID.
        /// </summary>
        /// <param name="id">The exam ID.</param>
        /// <returns>Delete result object.</returns>
        public async Task<DeleteResponseRoot> DeleteExamById(int id)
        {
            var exists = await _apiDbContext.Exams.AnyAsync(x => x.Id == id);
            if (exists)
            {
                var toDelete = new ExamDM { Id = id };
                _apiDbContext.Exams.Remove(toDelete);

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return new DeleteResponseRoot(true, $"Exam ID {id} deleted successfully.");
            }

            return new DeleteResponseRoot(false, "Exam not found.");
        }        

        #endregion Delete

    }
}
