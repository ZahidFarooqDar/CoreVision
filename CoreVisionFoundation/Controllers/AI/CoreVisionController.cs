using CoreVisionBAL.Projects.HuggingFace;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.HuggingFace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreVisionFoundation.Security;

namespace CoreVisionFoundation.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CoreVisionController : ControllerBase
    {
        public readonly HuggingfaceProcess _huggingfaceProcess;
        public CoreVisionController(HuggingfaceProcess huggingfaceProcess) 
        {
            _huggingfaceProcess = huggingfaceProcess;
        }

        [HttpPost("audio-transcription")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<AudioTranscriptionRequestSM>>> TranscribeAudioUsingHuggingFaceAsync([FromBody] ApiRequest<AudioTranscriptionRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.TranscribeAudioUsingHuggingFaceAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("image-to-text")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> ExtractTextFromImageAsync([FromBody] ApiRequest<ImageDataSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.ExtractTextFromImageAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("qna")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> ExtractResponseUsingDeepSeekAsync([FromBody] ApiRequest<TextRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.ExtractResponseUsingDeepSeekAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("translate")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> TranslateTextAsync([FromBody] ApiRequest<TranslationRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.TranslateTextAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("summary")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> SummarizeTextAsync([FromBody] ApiRequest<HuggingFaceRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.SummarizeTextAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("image-generation")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<Base64ImageResponseSM>>> GenerateHuggingImageAsync([FromBody] ApiRequest<HuggingFaceRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _huggingfaceProcess.GenerateHuggingImageAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }
    }
}
