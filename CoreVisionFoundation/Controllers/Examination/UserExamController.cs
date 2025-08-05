using CoreVisionBAL.Foundation.Web;
using CoreVisionBAL.General.Examination;
using CoreVisionDomainModels.v1.Examination;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.Examination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreVisionFoundation.Controllers.Examination
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
    public class UserExamController : ControllerBase
    {
        private readonly UserExamProcess _userExamProcess;

        public UserExamController(UserExamProcess userExamProcess)
        {
            _userExamProcess = userExamProcess;
        }

        #region Eligibility

        [HttpGet("eligible/subject")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> IsUserEligibleForSubjectTest([FromQuery] int subjectId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.IsUserEligibleForSubjectTest(subjectId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("eligible/exam")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> IsUserEligibleForExamTest([FromQuery] int examId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.IsUserEligibleForExamTest(examId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("eligible/topic")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> IsUserEligibleForSubjectTopicTest([FromQuery] int topicId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.IsUserEligibleForSubjectTopicTest(topicId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region User Subject Test

        [HttpPost("subjecttest/get")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> UserSubjectTestMarksGetRequest([FromQuery] int subjectId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserSubjectTestMarksGetRequest(subjectId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("subjecttest/submit")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<TestResponseSM>>> UserSubjectTestMarks([FromQuery] int subjectTestId,[FromBody] ApiRequest<List<MCQSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserSubjectTestMarks(subjectTestId, innerReq, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region User Exam Test

        [HttpPost("examtest/get")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> UserExamTestMarksGetRequest([FromQuery] int examId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserExamTestMarksGetRequest(examId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("examtest/submit")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<TestResponseSM>>> UserExamTestMarks([FromQuery] int examTestId,[FromBody] ApiRequest<List<MCQSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }

            var ret = await _userExamProcess.UserExamTestMarks(examTestId, innerReq, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region User Topic Test

        [HttpPost("topictest/get")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> UserTopicTestMarksGetRequest([FromQuery] int topicId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserTopicTestMarksGetRequest(topicId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPost("topictest/submit")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<TestResponseSM>>> UserTopicTestMarks([FromQuery] int topicTestId, [FromQuery] int topicId, [FromBody] ApiRequest<List<MCQSM>> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserTopicTestMarks(topicTestId, innerReq, topicId, userId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion

        #region Reports

        [HttpGet("report/exam")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<UserTestReportSM>>> UserExamTestReport([FromQuery] int examId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserExamTestReport(userId, examId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("report/subject")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<UserTestReportSM>>> UserSubjectTestReport([FromQuery] int subjectId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserSubjectTestReport(userId, subjectId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("report/topic")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema)]
        public async Task<ActionResult<ApiResponse<UserTestReportSM>>> UserTopicTestReport([FromQuery] int topicId)
        {
            int userId = User.GetUserRecordIdFromCurrentUserClaims();
            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid));
            }
            var ret = await _userExamProcess.UserTopicTestReport(userId, topicId);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion
    }
}
