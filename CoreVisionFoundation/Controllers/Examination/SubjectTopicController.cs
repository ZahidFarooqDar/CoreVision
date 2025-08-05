using CoreVisionBAL.Foundation.Web;
using CoreVisionBAL.General.Examination;
using CoreVisionFoundation.Controllers.Base;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.Examination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CoreVisionFoundation.Controllers.Examination
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectTopicController : ApiControllerWithOdataRoot<SubjectTopicSM>
    {
        private readonly SubjectTopicProcess _subjectTopicProcess;
        public SubjectTopicController(SubjectTopicProcess process)
            : base(process)
        {
            _subjectTopicProcess = process;
        }

        #region OData EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectTopicSM>>>> GetAsOdata(ODataQueryOptions<SubjectTopicSM> oDataOptions)
        {
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion OData

        #region Get All / By Id / By SubjectId and Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectTopicSM>>>> GetAll()
        {
            var retList = await _subjectTopicProcess.GetAllSubjectTopics();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("subject/{subjectId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectTopicSM>>>> GetAllTopicsBySubjectId(int subjectId)
        {
            var retList = await _subjectTopicProcess.GetAllTopicsBySubjectId(subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<SubjectTopicSM>>> GetById(int id)
        {
            var ret = await _subjectTopicProcess.GetSubjectTopicById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllSubjectTopicCount()
        {
            var ret = await _subjectTopicProcess.GetAllSubjectTopicCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Subject Topics")));
        }

        [HttpGet("subject/count/{subjectId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllTopicBySubjectIdCount(int subjectId)
        {
            var ret = await _subjectTopicProcess.GetAllTopicBySubjectIdCountResponse(subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Topics for Subject")));
        }

        #endregion

        #region Add / Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectTopicSM>>> Add([FromBody] ApiRequest<SubjectTopicSM> apiRequest, [FromQuery] int subjectId)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var ret = await _subjectTopicProcess.AddSubjectTopic(innerReq, subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectTopicSM>>> Update(int id, [FromBody] ApiRequest<SubjectTopicSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var ret = await _subjectTopicProcess.UpdateSubjectTopic(id, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(int id)
        {
            var ret = await _subjectTopicProcess.DeleteSubjectTopicById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion
    }
}
