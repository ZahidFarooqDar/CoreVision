using CoreVisionBAL.Foundation.Web;
using CoreVisionBAL.General.Examination;
using CoreVisionFoundation.Controllers.Base;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.Examination;
using CoreVisionServiceModels.v1.General.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CoreVisionFoundation.Controllers.Examination
{
    [Route("api/[controller]")]
    [ApiController]
    public class MCQController : ApiControllerWithOdataRoot<MCQSM>
    {
        private readonly MCQProcess _mcqProcess;
        public MCQController(MCQProcess process)
            : base(process)
        {
            _mcqProcess = process;
        }

        #region OData Endpoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetAsOdata(ODataQueryOptions<MCQSM> oDataOptions)
        {
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion OData

        #region Get All / By Id / By Exam, Subject, SubjectTopic / Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetAll()
        {
            var retList = await _mcqProcess.GetAllMCQs();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<MCQSM>>> GetById(int id)
        {
            var ret = await _mcqProcess.GetMCQById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("{id}/noanswers")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<MCQSM>>> GetByIdWithoutAnswers(int id)
        {
            var ret = await _mcqProcess.GetMCQByIdWithoutAnswers(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("exam/{examId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetMCQsOfExam(int examId, [FromQuery] int skip, [FromQuery] int top)
        {
            var retList = await _mcqProcess.GetMCQsOfExam(examId, skip, top);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("subject/{subjectId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetMCQsOfSubject(int subjectId, [FromQuery] int skip, [FromQuery] int top)
        {
            var retList = await _mcqProcess.GetMCQsOfSubject(subjectId, skip, top);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("subjecttopic/{subTopicId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetMCQsOfSubjectTopic(int subTopicId, [FromQuery] int skip, [FromQuery] int top)
        {
            var retList = await _mcqProcess.GetMCQsOfSubjectTopic(subTopicId, skip, top);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("random/subject/{subjectId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetRandomMCQsOfSubject(int subjectId)
        {
            var retList = await _mcqProcess.GetRandomMCQsOfSubject(subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("random/exam/{examId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetRandomMCQsOfExam(int examId)
        {
            var retList = await _mcqProcess.GetRandomMCQsOfExam(examId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("random/subjecttopic/{subjectTopicId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MCQSM>>>> GetRandomMCQsOfSubjectTopic(int subjectTopicId)
        {
            var retList = await _mcqProcess.GetRandomMCQsOfSubjectTopic(subjectTopicId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllMCQCount()
        {
            var ret = await _mcqProcess.GetAllMCQCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total MCQs")));
        }

        [HttpGet("count/exam/{examId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetMCQsOfExamCount(int examId)
        {
            var ret = await _mcqProcess.GetMCQsOfExamCount(examId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "MCQ Count for Exam")));
        }

        [HttpGet("count/subject/{subjectId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetMCQsOfSubjectCount(int subjectId)
        {
            var ret = await _mcqProcess.GetMCQsOfSubjectCount(subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "MCQ Count for Subject")));
        }

        [HttpGet("count/subjecttopic/{subjectTopicId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetMCQsOfSubjectTopicCount(int subjectTopicId)
        {
            var ret = await _mcqProcess.GetMCQsOfSubjectTopicsCount(subjectTopicId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "MCQ Count for SubjectTopic")));
        }

        #endregion

        #region Add/Update

        [HttpPost("exam")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> AddListOfMCQsForExams([FromBody] ApiRequest<List<MCQUploadRequestSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var ret = await _mcqProcess.AddListOfMSQsForExams(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("subject")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> AddListOfMCQsForSubjects([FromBody] ApiRequest<List<MCQUploadRequestSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var ret = await _mcqProcess.AddListOfMSQsForSubjects(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("subjecttopic")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> AddListOfMCQsForSubjectTopics([FromBody] ApiRequest<List<MCQUploadRequestSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var ret = await _mcqProcess.AddListOfMSQsForSubjectTopics(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<MCQSM>>> Update(int id, [FromBody] ApiRequest<MCQSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var ret = await _mcqProcess.UpdateMCQ(id, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region MCQ Answer and Validation

        [HttpGet("{id}/answer")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<MCQAnswerSM>>> GetMCQAnswer(int id)
        {
            var ret = await _mcqProcess.CorrectAnswer(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("{id}/validate")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ValidateAnswer(int id, [FromBody] ApiRequest<string> answerRequest)
        {
            var answer = answerRequest?.ReqData;
            if (string.IsNullOrWhiteSpace(answer))
            {
                return BadRequest(ModelConverter.FormNewErrorResponse("Answer is required", ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var ret = await _mcqProcess.ValidateAnswer(id, answer);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }
        
        [HttpPost("{id}/explination")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin,ClientEmployee")]        
        public async Task<ActionResult<ApiResponse<AITextResponse>>> Explain(int id)
        {            
            var ret = await _mcqProcess.QuestionAnswerExplinationByAI(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(int id)
        {
            var ret = await _mcqProcess.DeleteMCQById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion
    }
}
