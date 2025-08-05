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
    public class ExamController : ApiControllerWithOdataRoot<ExamSM>
    {

        private readonly ExamProcess _examProcess;
        public ExamController(ExamProcess process)
            : base(process)
        {
            _examProcess = process;
        }

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAsOdata(ODataQueryOptions<ExamSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata

        #region Get All/By Id and Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAll()
        {
            var retList = await _examProcess.GetAllExams();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("active")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSM>>>> GetAllActiveExams()
        {
            var retList = await _examProcess.GetAllActiveExams();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> GetById(int id)
        {
            var ret = await _examProcess.GetExamById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllExamsCount()
        {
            var ret = await _examProcess.GetAllExamsCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Active Exams")));
        }

        [HttpGet("active/count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllActiveExamsCount()
        {
            var ret = await _examProcess.GetAllActiveExamsCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total Actice Exams")));
        }

        #endregion Get

        #region Add/Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> Add([FromBody] ApiRequest<ExamSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _examProcess.AddExam(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> Update(int id, [FromBody] ApiRequest<ExamSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _examProcess.UpdateExam(id, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("status/{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<ExamSM>>> UpdateExamStatus(int id, bool status)
        {
            #region Check Request
            
            #endregion Check Request

            var ret = await _examProcess.UpdateExamStatus(id, status);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Add/Update

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Delete(int id)
        {
            var ret = await _examProcess.DeleteExamById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Delete

    }
}
