﻿using CoreVisionBAL.Projects.AzureAI;
using CoreVisionBAL.Projects.BaseAIProcess;
using CoreVisionBAL.Projects.HuggingFace;
using CoreVisionBAL.Projects.StoryAI;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.AzureAI;
using CoreVisionServiceModels.v1.General.HuggingFace;
using CoreVisionServiceModels.v1.General.StoryAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreVisionFoundation.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CoreVisionController : ControllerBase
    {
        public readonly HuggingfaceProcess _huggingfaceProcess;
        public readonly BaseAIProcess _baseAIProcess;
        private readonly AzureAIProcess _azureAIProcess;
        private readonly StoryProcess _storyProcess;
        public CoreVisionController(HuggingfaceProcess huggingfaceProcess, BaseAIProcess baseAIProcess, StoryProcess storyProcess, AzureAIProcess azureAIProcess) 
        {
            _huggingfaceProcess = huggingfaceProcess;
            _baseAIProcess = baseAIProcess;
            _storyProcess = storyProcess;
            _azureAIProcess = azureAIProcess;
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
            var resp = await _baseAIProcess.BaseMethodForTextExtraction(innerReq);
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
            var resp = await _baseAIProcess.BaseMethodForTextTranslation(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("short-summary")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> SummarizeTextAsync([FromBody] ApiRequest<AzureAIRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _baseAIProcess.BaseMethodForShortSummarization(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }

        [HttpPost("descriptive-summary")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<HuggingFaceResponseSM>>> DescriptiveSummarizeTextAsync([FromBody] ApiRequest<AzureAIRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _baseAIProcess.BaseMethodForExtensiveSummarization(innerReq);
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

        [HttpPost("generate-story")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<ContentGenerationResponseSM>>> GenerateStoryAsync([FromBody] ApiRequest<ContentGenerationRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _storyProcess.GenerateStory(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));

        }
    }
}
