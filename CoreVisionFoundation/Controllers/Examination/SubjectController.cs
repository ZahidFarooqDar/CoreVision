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
    public class SubjectController : ApiControllerWithOdataRoot<SubjectSM>
    {

        private readonly SubjectProcess _subjectProcess;
        public SubjectController(SubjectProcess process)
            : base(process)
        {
            _subjectProcess = process;
        }

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAsOdata(ODataQueryOptions<SubjectSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata

        #region Get All/By Id and Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAll()
        {
            var retList = await _subjectProcess.GetAllSubjects();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("exam/{examId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectSM>>>> GetAllSubjectsOfParticularExam(int examId)
        {
            var retList = await _subjectProcess.GetAllSubjectsOfExams(examId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }



        [HttpGet]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> GetById(int id)
        {
            var ret = await _subjectProcess.GetSubjectById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllSubjectsCount()
        {
            var ret = await _subjectProcess.GetAllSubjectsCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Active Exams")));
        }

        [HttpGet("exam/count/{examId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllExamSubjectsCount(int examId)
        {
            var ret = await _subjectProcess.GetAllSubjectsOfExamsCountResponse(examId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Active Exams")));
        }



        #endregion Get

        #region Add/Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> Add([FromBody] ApiRequest<SubjectSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _subjectProcess.AddSubject(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("list")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> AddListofSubjects([FromBody] ApiRequest<List<SubjectSM>> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _subjectProcess.AddListOfSubjects(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("assign")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> AssignSubjectToExam([FromBody] ApiRequest<ExamSubjectsSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _subjectProcess.AssignSubjectToExam(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<SubjectSM>>> Update(int id, [FromBody] ApiRequest<SubjectSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _subjectProcess.UpdateSubject(id, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }        

        #endregion Add/Update

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Delete(int id)
        {
            var ret = await _subjectProcess.DeleteSubjectById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Delete

    }
}
