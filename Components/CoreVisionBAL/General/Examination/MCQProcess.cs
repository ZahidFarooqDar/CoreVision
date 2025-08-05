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
    public class MCQProcess : CoreVisionBalOdataBase<MCQSM>
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor

        public MCQProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata

        public override async Task<IQueryable<MCQSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.MCQs;
            IQueryable<MCQSM> retSM = await base.MapEntityAsToQuerable<MCQDM, MCQSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region Count

        #region Exam, Subject, Subject Topic MCQs Count

        public async Task<int> GetMCQsOfExamCount(int examId)
        {
            var count = await _apiDbContext.MCQs
                .Where(x => x.ExamId == examId)
                .CountAsync();

            return count;
        }
        public async Task<int> GetMCQsOfSubjectCount(int subjectId)
        {
            var count = await _apiDbContext.MCQs
                .Where(x => x.SubjectId == subjectId)
                .CountAsync();         

            return count;
        }

        public async Task<int> GetMCQsOfSubjectTopicsCount(int subjectTopicId)
        {
            var count = await _apiDbContext.MCQs
                .Where(x => x.SubjectTopicId == subjectTopicId)
                .CountAsync();

            return count;
        }

        #endregion Exam, Subject, Subject Topic MCQs Count
        public async Task<int> GetAllMCQCountResponse()
        {
            return await _apiDbContext.MCQs.AsNoTracking().CountAsync();
        }

        #endregion Count

        #region Get All

        #region Get All
        public async Task<List<MCQSM>?> GetAllMCQs()
        {
            var list = await _apiDbContext.MCQs.ToListAsync();
            return list == null ? null : _mapper.Map<List<MCQSM>>(list);
        }
        #endregion Get All

        #region Get By Exam, Subject, Subject Topic Id With count

        public async Task<List<MCQSM>?> GetMCQsOfSubject(int subjectId, int skip, int top)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.SubjectId == subjectId)
                .Skip(skip).Take(top)
                .ToListAsync();

            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }

            var result = _mapper.Map<List<MCQSM>>(list);           

            return result;
        }

        public async Task<List<MCQSM>?> GetMCQsOfExam(int examId, int skip, int top)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.ExamId == examId)
                .Skip(skip).Take(top)
                .ToListAsync();
            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }
            var result = _mapper.Map<List<MCQSM>>(list);           

            return result;
        }

        public async Task<List<MCQSM>?> GetMCQsOfSubjectTopic(int subTopicId, int skip, int top)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.SubjectTopicId == subTopicId)
                .ToListAsync();

            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }
            var result = _mapper.Map<List<MCQSM>>(list);

            return result;
        }

        #endregion Get By Exam, Subject, Subject Topic Id With count

        #region Get Random 50 MCQs

        public async Task<List<MCQSM>?> GetRandomMCQsOfSubject(int subjectId)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.SubjectId == subjectId)
                .OrderBy(x => Guid.NewGuid())
                .Take(50)
                .ToListAsync();
            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }
            var result = _mapper.Map<List<MCQSM>>(list);
            foreach (var item in result)
            {
                item.CorrectOption = string.Empty;
                item.Explanation = string.Empty;
            }

            return result;
        }

        public async Task<List<MCQSM>?> GetRandomMCQsOfExam(int examId)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.ExamId == examId)
                .OrderBy(x => Guid.NewGuid())
                .Take(50)
                .ToListAsync();
            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }
            var result = _mapper.Map<List<MCQSM>>(list);
            foreach (var item in result)
            {
                item.CorrectOption = string.Empty;
                item.Explanation = string.Empty;
            }

            return result;
        }

        public async Task<List<MCQSM>?> GetRandomMCQsOfSubjectTopic(int subTopicId)
        {
            var list = await _apiDbContext.MCQs
                .Where(x => x.SubjectTopicId == subTopicId)
                .OrderBy(x => Guid.NewGuid())
                .Take(50)
                .ToListAsync();
            if (list.Count == 0)
            {
                return new List<MCQSM>();
            }
            var result = _mapper.Map<List<MCQSM>>(list);

            foreach (var item in result)
            {
                item.CorrectOption = string.Empty;
                item.Explanation = string.Empty;
            }

            return result;
        }

        #endregion Get Random 50 MCQs


        #endregion Get All

        #region Get Single
        public async Task<MCQSM?> GetMCQById(int id)
        {
            var dm = await _apiDbContext.MCQs.FindAsync(id);
            var sm = _mapper.Map<MCQSM>(dm);
            return dm == null ? null : sm;
        }

        public async Task<MCQSM?> GetMCQByIdWithoutAnswers(int id)
        {
            var dm = await _apiDbContext.MCQs.FindAsync(id);
            var sm = _mapper.Map<MCQSM>(dm);
            sm.CorrectOption = string.Empty;
            sm.Explanation = string.Empty;
            return dm == null ? null : sm;
        }

        #endregion Get Single

        #region Add

        #region Add MCQ List For Exams
        
        public async Task<BoolResponseRoot> AddListOfMSQsForExams(List<MCQUploadRequestSM> objSM)
        {
            if (objSM.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Failed to add exam MCQs, Data not found to add", "Something went wrong while adding new Exam MCQs");
            }

            foreach (var item in objSM)
            {
                var request = _mapper.Map<MCQSM>(item);
                await AddMCQForExam(request, item.Id);
            }
            return new BoolResponseRoot(true, "Exams MCQs added successfully.");

        }

        public async Task<MCQSM?> AddMCQForExam(MCQSM objSM, int examId)
        {
            if (objSM == null || examId < 1)
            {
                return null;
            }

            var dm = _mapper.Map<MCQDM>(objSM);
            dm.ExamId = examId;
            dm.SubjectTopicId = null;
            dm.SubjectId = null;
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.MCQs.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetMCQById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                $"Failed to add MCQ for ExamId: {examId}",
                "Something went wrong while adding new MCQ");
        }

        #endregion Add MCQ List

        #region Add MCQ For Subjects

        public async Task<BoolResponseRoot> AddListOfMSQsForSubjects(List<MCQUploadRequestSM> objSM)
        {
            if (objSM.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Failed to add Subject MCQs, Data not found to add", "Something went wrong while adding new Subject MCQs");
            }

            foreach (var item in objSM)
            {
                var request = _mapper.Map<MCQSM>(item);
                await AddMCQForSubject(request, item.Id);
            }
            return new BoolResponseRoot(true, "Subjects MCQS added successfully.");
        }

        public async Task<MCQSM?> AddMCQForSubject(MCQSM objSM, int subjectId)
        {
            if (objSM == null || subjectId < 1)
            {
                return null;
            }

            var dm = _mapper.Map<MCQDM>(objSM);
            dm.ExamId = null;
            dm.SubjectTopicId = null;
            dm.SubjectId = subjectId;
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.MCQs.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetMCQById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                $"Failed to add MCQ for ExamId: {subjectId}",
                "Something went wrong while adding new MCQ");
        }

        #endregion Add MCQ For Subjects

        #region Add MCQ For Subject Topic

        public async Task<BoolResponseRoot> AddListOfMSQsForSubjectTopics(List<MCQUploadRequestSM> objSM)
        {
            if (objSM.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Failed to add Subject topic MCQs, Data not found to add", "Something went wrong while adding new Topic Topic MCQs");
            }

            foreach (var item in objSM)
            {
                var request = _mapper.Map<MCQSM>(item);
                await AddMCQForSubjectTopic(request, item.Id);
            }
            return new BoolResponseRoot(true, "Subjects Topic MCQs added successfully.");
        }
        public async Task<MCQSM?> AddMCQForSubjectTopic(MCQSM objSM, int subjectTopicId)
        {
            if (objSM == null || subjectTopicId < 1)
            {
                return null;
            }

            var dm = _mapper.Map<MCQDM>(objSM);
            dm.ExamId = null;
            dm.SubjectTopicId = subjectTopicId;
            dm.SubjectId = null;
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.MCQs.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetMCQById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                $"Failed to add MCQ for ExamId: {subjectTopicId}",
                "Something went wrong while adding new MCQ");
        }

        #endregion Add MCQ For Subject Topic

        #endregion Add

        #region Answer For MCQ
        public async Task<BoolResponseRoot> ValidateAnswer(int mcqId, string answer)
        {
            var response = await IsAnswerCorrect(mcqId, answer);
            return new BoolResponseRoot(response, response ? "Answer is correct" : "Answer is incorrect");
        }
        public async Task<bool> IsAnswerCorrect(int mcqId, string answer)
        {
            if (mcqId < 1 || string.IsNullOrWhiteSpace(answer))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Invalid MCQId or Answer", "Invalid MCQId or Answer");
            }

            var dm = await _apiDbContext.MCQs.FindAsync(mcqId);
            if (dm == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "MCQ not found", "MCQ not found");
            }

            var response = string.Equals(dm.CorrectOption.Trim(), answer.Trim(), StringComparison.OrdinalIgnoreCase);
            return response;
        }

        public async Task<MCQAnswerSM?> CorrectAnswer(int id)
        {
            var dm = await _apiDbContext.MCQs.FindAsync(id);
            if(dm == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "MCQ not found", "MCQ not found");
            }
            return new MCQAnswerSM
            {
                CorrectOption = dm.CorrectOption,
                Explanation = dm.Explanation
            };
        }

        #endregion Answer For MCQ

        #region Update

        public async Task<MCQSM?> UpdateMCQ(int objIdToUpdate, MCQSM objSM)
        {
            if (objSM != null && objIdToUpdate > 0)
            {
                var objDM = await _apiDbContext.MCQs.FindAsync(objIdToUpdate);
                if (objDM != null)
                {
                    objSM.Id = objIdToUpdate;
                    _mapper.Map(objSM, objDM);
                    objDM.LastModifiedBy = _loginUserDetail.LoginId;
                    objDM.LastModifiedOnUTC = DateTime.UtcNow;

                    if (await _apiDbContext.SaveChangesAsync() > 0)
                        return await GetMCQById(objIdToUpdate);

                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Error updating MCQ ID:{objIdToUpdate}", "Update failed.");
                }

                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"MCQ ID:{objIdToUpdate} not found.", "MCQ not found.");
            }

            return null;
        }        

        #endregion Update

        #region Delete
        public async Task<DeleteResponseRoot> DeleteMCQById(int id)
        {
            var exists = await _apiDbContext.MCQs.AnyAsync(x => x.Id == id);
            if (exists)
            {
                var toDelete = new MCQDM { Id = id };
                _apiDbContext.MCQs.Remove(toDelete);

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return new DeleteResponseRoot(true, $"MCQ ID {id} deleted successfully.");
            }

            return new DeleteResponseRoot(false, "MCQ not found.");
        }

        #endregion Delete
    }
}
