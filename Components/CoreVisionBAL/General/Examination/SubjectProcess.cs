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
    public class SubjectProcess : CoreVisionBalOdataBase<SubjectSM>
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor

        public SubjectProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata

        public override async Task<IQueryable<SubjectSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Subjects;
            IQueryable<SubjectSM> retSM = await base.MapEntityAsToQuerable<SubjectDM, SubjectSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region Count

        public async Task<int> GetAllSubjectsCountResponse()
        {
            return _apiDbContext.Subjects.AsNoTracking().Count();
        }
        public async Task<int> GetAllSubjectsOfExamsCountResponse(int examId)
        {
            if (examId < 1)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Invalid exam id, Id: {examId}", "Something went wrong, Please try again later");
            }
            var subjectIdsCount = await _apiDbContext.ExamSubjects.AsNoTracking().Where(x => x.ExamId == examId).Select(x => x.SubjectId).Distinct().CountAsync();

            return subjectIdsCount;
        }

        #endregion Count

        #region Get All

        public async Task<List<SubjectSM>?> GetAllSubjects()
        {
            var list = await _apiDbContext.Subjects.ToListAsync();
            if (list == null)
                return null;

            return _mapper.Map<List<SubjectSM>>(list);
        }

        public async Task<List<SubjectSM>?> GetAllSubjectsOfExams(int examId)
        {
            if (examId < 1)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Invalid exam id, Id: {examId}", "Something went wrong, Please try again later");
            }
            var subjectIds = await _apiDbContext.ExamSubjects.AsNoTracking().Where(x => x.ExamId == examId).Select(x => x.SubjectId).Distinct().ToListAsync();

            var list = await _apiDbContext.Subjects.Where(x => subjectIds.Contains(x.Id)).ToListAsync();
            if (list.Count == 0)
            {
                return new List<SubjectSM>();
            }               

            var response = _mapper.Map<List<SubjectSM>>(list);
            return response;
        }

        #endregion Get All

        #region Get Single

        public async Task<SubjectSM?> GetSubjectById(int id)
        {
            var dm = await _apiDbContext.Subjects.FindAsync(id);
            if (dm == null)
                return null;

            return _mapper.Map<SubjectSM>(dm);
        }

        #endregion Get Single
            
        #region Add

        public async Task<SubjectSM?> AddSubject(SubjectSM objSM)
        {
            if (objSM == null)
                return null;

            var dm = _mapper.Map<SubjectDM>(objSM);
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.Subjects.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetSubjectById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add subject. See inner exception.",
                "Something went wrong while adding new Subject");
        }

        public async Task<BoolResponseRoot> AddListOfSubjects(List<SubjectSM> objSM)
        {
            if (objSM.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Failed to add subjects, Data not found to add", "Something went wrong while adding new Subjects");
            }

            foreach (var item in objSM)
            {
                await AddSubject(item);
            }
            return new BoolResponseRoot(true, "Subjects added successfully.");

        }

        #endregion Add

        #region Assign Subject to Exam

        public async Task<BoolResponseRoot> AssignSubjectToExam(ExamSubjectsSM sm)
        {
            var existingSubject = await _apiDbContext.Subjects.FindAsync(sm.SubjectId);
            if (existingSubject == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject ID:{sm.SubjectId} not found.", "Subject not found.");
            }
            var existingExam = await _apiDbContext.Exams.FindAsync(sm.ExamId);
            if (existingExam == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exam ID:{sm.ExamId} not found.", "Exam not found.");
            }
            var existingSubjectInExam = await _apiDbContext.ExamSubjects.Where(x => x.SubjectId == sm.SubjectId && x.ExamId == sm.ExamId).FirstOrDefaultAsync();
            if (existingSubjectInExam != null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject ID:{sm.SubjectId} already assigned to Exam ID:{sm.ExamId}.", "Subject already assigned to Exam.");
            }
            var dm = new ExamSubjectsDM()
            {
                SubjectId = sm.SubjectId,
                ExamId = sm.ExamId,
            };
            await _apiDbContext.ExamSubjects.AddAsync(dm);
            if(await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new BoolResponseRoot(true, "Subject assigned to Exam successfully.");
            }
            return new BoolResponseRoot(false, "Failed to assign Subject to Exam.");
        }

        #endregion Assign Subject to Exam

        #region Update

        public async Task<SubjectSM?> UpdateSubject(int objIdToUpdate, SubjectSM objSM)
        {
            if (objSM != null && objIdToUpdate > 0)
            {
                var objDM = await _apiDbContext.Subjects.FindAsync(objIdToUpdate);
                if (objDM != null)
                {
                    objSM.Id = objIdToUpdate;
                    _mapper.Map(objSM, objDM);
                    objDM.LastModifiedBy = _loginUserDetail.LoginId;
                    objDM.LastModifiedOnUTC = DateTime.UtcNow;

                    if (await _apiDbContext.SaveChangesAsync() > 0)
                        return await GetSubjectById(objIdToUpdate);

                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Error updating Subject ID:{objIdToUpdate}", "Update failed.");
                }
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject ID:{objIdToUpdate} not found.", "Subject not found.");
            }

            return null;
        }

        public async Task<SubjectSM?> UpdateSubjectStatus(int objIdToUpdate)
        {
            var objDM = await _apiDbContext.Subjects.FindAsync(objIdToUpdate);
            if (objDM != null)
            {
                objDM.LastModifiedBy = _loginUserDetail.LoginId;
                objDM.LastModifiedOnUTC = DateTime.UtcNow;

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return await GetSubjectById(objIdToUpdate);

                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Failed to update status for Subject ID:{objIdToUpdate}", "Status update failed.");
            }

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject ID:{objIdToUpdate} not found.", "Subject not found.");
        }

        #endregion Update

        #region Delete

        public async Task<DeleteResponseRoot> DeleteSubjectById(int id)
        {
            var exists = await _apiDbContext.Subjects.AnyAsync(x => x.Id == id);
            if (exists)
            {
                var toDelete = new SubjectDM { Id = id };
                _apiDbContext.Subjects.Remove(toDelete);

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return new DeleteResponseRoot(true, $"Subject ID {id} deleted successfully.");
            }

            return new DeleteResponseRoot(false, "Subject not found.");
        }

        #endregion Delete
    }
}
