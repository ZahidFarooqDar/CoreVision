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
using System.Collections.Generic;

namespace CoreVisionBAL.General.Examination
{
    public class SubjectTopicProcess : CoreVisionBalOdataBase<SubjectTopicSM>
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;

        #endregion Properties

        #region Constructor

        public SubjectTopicProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #endregion Constructor

        #region Odata

        public override async Task<IQueryable<SubjectTopicSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.SubjectTopics;
            IQueryable<SubjectTopicSM> retSM = await base.MapEntityAsToQuerable<SubjectTopicDM, SubjectTopicSM>(_mapper, entitySet);
            return retSM;
        }

        #endregion Odata

        #region Count

        public async Task<int> GetAllSubjectTopicCountResponse()
        {
            return await _apiDbContext.SubjectTopics.AsNoTracking().CountAsync();
        }
        public async Task<int> GetAllTopicBySubjectIdCountResponse(int subjectId)
        {
            var count = await _apiDbContext.SubjectTopics.AsNoTracking().Where(x => x.SubjectId == subjectId).CountAsync();
            return count;
        }

        #endregion Count

        #region Get All

        public async Task<List<SubjectTopicSM>?> GetAllSubjectTopics()
        {
            var list = await _apiDbContext.SubjectTopics.AsNoTracking().ToListAsync();
            return list == null ? null : _mapper.Map<List<SubjectTopicSM>>(list);
        }

        public async Task<List<SubjectTopicSM>?> GetAllTopicsBySubjectId(int subjectId)
        {
            var list = await _apiDbContext.SubjectTopics.AsNoTracking().Where(x => x.SubjectId == subjectId).ToListAsync();
            if(list.Count == 0)
            {
                return new List<SubjectTopicSM>();
            }
            return _mapper.Map<List<SubjectTopicSM>>(list);
        }

        #endregion Get All

        #region Get Single

        public async Task<SubjectTopicSM?> GetSubjectTopicById(int id)
        {
            var dm = await _apiDbContext.SubjectTopics.FindAsync(id);
            return dm == null ? null : _mapper.Map<SubjectTopicSM>(dm);
        }

        #endregion Get Single

        #region Add

        public async Task<SubjectTopicSM?> AddSubjectTopic(SubjectTopicSM objSM, int subjectId)
        {
            if (objSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,"SubjectTopic is null", "Please provide valid Subject Topic.");
            }
            var existingSubjectTopic = await _apiDbContext.Subjects.FindAsync(subjectId);
            if(existingSubjectTopic == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject ID:{subjectId} not found.", "Subject not found.");
            }

            var dm = _mapper.Map<SubjectTopicDM>(objSM);
            dm.Id = dm.Id;
            dm.SubjectId = subjectId;
            dm.CreatedBy = _loginUserDetail.LoginId;
            dm.CreatedOnUTC = DateTime.UtcNow;

            await _apiDbContext.SubjectTopics.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
                return await GetSubjectTopicById(dm.Id);

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add Subject Topic",
                "Something went wrong while adding new Subject Topic");
        }

        #endregion Add

        #region Update

        public async Task<SubjectTopicSM?> UpdateSubjectTopic(int objIdToUpdate, SubjectTopicSM objSM)
        {
            if (objSM != null && objIdToUpdate <= 0)
            {
                var objDM = await _apiDbContext.SubjectTopics.FindAsync(objIdToUpdate);
                if (objDM != null)
                {
                    objSM.Id = objIdToUpdate;
                    _mapper.Map(objSM, objDM);
                    objDM.LastModifiedBy = _loginUserDetail.LoginId;
                    objDM.LastModifiedOnUTC = DateTime.UtcNow;

                    if (await _apiDbContext.SaveChangesAsync() > 0)
                        return await GetSubjectTopicById(objIdToUpdate);

                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Error updating SubjectTopic ID:{objIdToUpdate}", "Update failed.");
                }

                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject Topic ID:{objIdToUpdate} not found.", "SubjectTopic not found.");
            }

            return null;
        }

        #endregion Update

        #region Delete

        public async Task<DeleteResponseRoot> DeleteSubjectTopicById(int id)
        {
            var exists = await _apiDbContext.SubjectTopics.AnyAsync(x => x.Id == id);
            if (exists)
            {
                var toDelete = new SubjectTopicDM { Id = id };
                _apiDbContext.SubjectTopics.Remove(toDelete);

                if (await _apiDbContext.SaveChangesAsync() > 0)
                    return new DeleteResponseRoot(true, $"Subject Topic ID {id} deleted successfully.");
            }

            return new DeleteResponseRoot(false, "Subject Topic not found.");
        }

        #endregion Delete
    }
}
