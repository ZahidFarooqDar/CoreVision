using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionBAL.Projects.AzureAI;
using CoreVisionBAL.Projects.HuggingFace;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.AzureAI;
using CoreVisionServiceModels.v1.General.HuggingFace;

namespace CoreVisionBAL.Projects.BaseAIProcess
{
    public class BaseAIProcess : CoreVisionBalBase
    {
        private readonly APIConfiguration _configuration;
        private readonly HuggingfaceProcess _huggingfaceProcess;
        private readonly AzureAIProcess _azureAIProcess;
        private readonly string _huggingfaceProcessingModel;
        private readonly string _azureProcessingModel;
        public BaseAIProcess(IMapper mapper, ApiDbContext apiDbContext, APIConfiguration configuration, HuggingfaceProcess huggingfaceProcess, AzureAIProcess azureAIProcess)
            :base(mapper, apiDbContext)
        {
            _configuration = configuration;
            _azureAIProcess = azureAIProcess;
            _huggingfaceProcess = huggingfaceProcess;
            _azureProcessingModel = configuration.ExternalIntegrations.AzureConfiguration.AzureProcessingModel;
            _huggingfaceProcessingModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.HuggingfaceProcessingModel;
        }

        #region Text Summerization Base Method

        public async Task<AzureAIResponseSM> BaseMethodForShortSummarization(AzureAIRequestSM objSM)
        {
            if (string.IsNullOrWhiteSpace(_azureProcessingModel))
            {
                throw new CoreVisionException(
                    ApiErrorTypeSM.Fatal_Log,
                    "The AI model used for summarization is not specified.",
                    "Please specify a valid AI model for summarization."
                );
            }

            switch (_azureProcessingModel)
            {
                case "AzureAI":
                    return await _azureAIProcess.TextSummarizeAsync(objSM);
                case "HuggingFaceAI":
                    var input = new HuggingFaceRequestSM()
                    {
                        InputRequest = objSM.TextInput
                    };
                    var res = await _huggingfaceProcess.SummarizeTextAsync(input);
                    if (res != null)
                    {
                        return new AzureAIResponseSM()
                        {
                            TextResponse = res.Response
                        };
                    }
                    return null;

                default:
                    throw new CoreVisionException(
                        ApiErrorTypeSM.Fatal_Log,
                        "Unsupported AI model",
                        $"The AI model '{_azureProcessingModel}' is not supported."
                    );
            }
        }


        public async Task<AzureAIResponseSM> BaseMethodForExtensiveSummarization(AzureAIRequestSM objSM)
        {
            if (string.IsNullOrWhiteSpace(_azureProcessingModel))
            {
                throw new CoreVisionException(
                    ApiErrorTypeSM.Fatal_Log,
                    "The AI model used for summarization is not specified.",
                    "Please specify a valid AI model for summarization."
                );
            }

            switch (_azureProcessingModel)
            {
                case "AzureAI":
                    return await _azureAIProcess.ExtensiveSummarizeAsync(objSM);
                case "HuggingFaceAI":
                    var input = new HuggingFaceRequestSM()
                    {
                        InputRequest = objSM.TextInput
                    };
                    var res = await _huggingfaceProcess.SummarizeTextAsync(input);
                    if (res != null)
                    {
                        return new AzureAIResponseSM()
                        {
                            TextResponse = res.Response
                        };
                    }
                    return null;

                default:
                    throw new CoreVisionException(
                        ApiErrorTypeSM.Fatal_Log,
                        "Unsupported AI model",
                        $"The AI model '{_azureProcessingModel}' is not supported."
                    );
            }
        }


        #endregion Text Summerization Base Method

        #region Text Extraxtion Base Model

        public async Task<AzureAIResponseSM> BaseMethodForTextExtraction(ImageDataSM objSM)
        {
            if (string.IsNullOrWhiteSpace(_azureProcessingModel))
            {
                throw new CoreVisionException(
                    ApiErrorTypeSM.Fatal_Log,
                    "The AI model used for summarization is not specified.",
                    "Please specify a valid AI model for summarization."
                );
            }

            switch (_azureProcessingModel)
            {
                case "AzureAI":
                    return await _azureAIProcess.ExtractTextFromBase64ImageAsync(objSM);
                case "HuggingFaceAI":
                    var res = await _huggingfaceProcess.ExtractTextFromImageAsync(objSM);
                    return new AzureAIResponseSM()
                    {
                        TextResponse = res.TextResponse
                    };


                default:
                    throw new CoreVisionException(
                        ApiErrorTypeSM.Fatal_Log,
                        "Unsupported AI model",
                        $"The AI model '{_azureProcessingModel}' is not supported."
                    );
            }
        }

        #endregion Text Extraxtion Base Model

        #region Text Translation Base Method

        public async Task<AzureAIResponseSM> BaseMethodForTextTranslation(TranslationRequestSM objSM)
        {
            if (string.IsNullOrWhiteSpace(_azureProcessingModel))
            {
                throw new CoreVisionException(
                    ApiErrorTypeSM.Fatal_Log,
                    "The AI model used for summarization is not specified.",
                    "Please specify a valid AI model for summarization."
                );
            }

            switch (_azureProcessingModel)
            {
                case "AzureAI":
                    return await _azureAIProcess.TranslateTextAsync(objSM);
                case "HuggingFaceAI":
                    var input = new TranslationRequestSM()
                    {
                        Text = objSM.Text,
                        Language = objSM.Language
                    };
                    var res = await _huggingfaceProcess.TranslateTextAsync(input);
                    if (res != null)
                    {
                        return new AzureAIResponseSM()
                        {
                            TextResponse = res.TextResponse
                        };
                    }
                    return null;

                default:
                    throw new CoreVisionException(
                        ApiErrorTypeSM.Fatal_Log,
                        "Unsupported AI model",
                        $"The AI model '{_azureProcessingModel}' is not supported."
                    );
            }
        }

        #endregion Text Translation Base Method
    }
}
