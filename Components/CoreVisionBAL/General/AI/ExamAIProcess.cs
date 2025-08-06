using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.Examination;
using CoreVisionServiceModels.v1.General.AI;
using CoreVisionServiceModels.v1.General.HuggingFace;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CoreVisionBAL.General.AI
{
    public class ExamAIProcess : CoreVisionBalBase
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;
        private readonly APIConfiguration _apiConfiguration;
        private readonly PromptProcess _promptProcess;
        private readonly HttpClient _httpClient;
        private readonly int _port;
        private readonly string _huggingFaceBaseUrl;
        private readonly string _huggingFaceApiKey;
        private readonly string _transcriptionModel;
        private readonly string _summarizeModel;
        private readonly string _imageToTextModel;
        private readonly string _textToImageModel;
        private readonly string _minuteOfMeting;
        private readonly string _translationModel;
        private readonly string _deepSeekModel;
        private readonly string _languageDetectionModel;
        private readonly string _entitiesDetectionModel;
        private readonly bool _isHuggingFaceTestingMode;
        private readonly string _cohereApiKey;
        private readonly string _cohereBaseUrl;
        private readonly string _cohereSummerizeModel;
        private readonly string _cohereTranslationModel;
        private readonly bool _isCohereTestingMode;

        #endregion Properties

        #region Constructor
        public ExamAIProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext, APIConfiguration configuration,HttpClient httpClient, PromptProcess promptProcess)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
            _apiConfiguration = configuration;
            _httpClient = httpClient;
            _promptProcess = promptProcess;
            _huggingFaceBaseUrl = configuration.ExternalIntegrations.HuggingFaceConfiguration.BaseUrl;
            _huggingFaceApiKey = configuration.ExternalIntegrations.HuggingFaceConfiguration.ApiKey;
            _transcriptionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TranscriptionModel;
            //_httpClient.DefaultRequestHeaders.Add($"Authorization", $"Bearer {_huggingFaceApiKey}"); //Hugging Face            
            _summarizeModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.SummarizeModel;
            _minuteOfMeting = configuration.ExternalIntegrations.HuggingFaceConfiguration.MinuteOfMeeting;
            _translationModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TranslationModel;
            _languageDetectionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.LanguageDetectionModel;
            _entitiesDetectionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.EntitiesDetectionModel;
            _imageToTextModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.ImageToTextModel;
            _textToImageModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TextToImageModel;
            _deepSeekModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.DeepSeekModel;
            _isHuggingFaceTestingMode = configuration.ExternalIntegrations.HuggingFaceConfiguration.IsTestingMode;
            _cohereApiKey = configuration.ExternalIntegrations.CohereConfiguration.ApiKey;
            _cohereBaseUrl = configuration.ExternalIntegrations.CohereConfiguration.BaseUrl;
            _cohereSummerizeModel = configuration.ExternalIntegrations.CohereConfiguration.SummarizeModel;
            _cohereTranslationModel = configuration.ExternalIntegrations.CohereConfiguration.TranslationModel;
            _isCohereTestingMode = configuration.ExternalIntegrations.CohereConfiguration.IsTestingMode;
        }

        #endregion Constructor

        #region MCQ Explination
        public async Task<AITextResponse> GenerateMCQExplination(MCQSM objSM)
        {            
            if(objSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,"No input data found","No input data found");
            }
            var prompt = _promptProcess.GeneratePromptForMCQ(objSM);

            var apiUrl = _cohereBaseUrl;
            var key = _cohereApiKey;

            var requestBody = new
            {
                model = _cohereSummerizeModel,
                messages = new[] {
                    new {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");

            int maxRetries = 3;
            int attempt = 0;
            while (attempt < maxRetries)
            {

                try
                {
                    // Make the request
                    var response = await _httpClient.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        if (responseJson?.message?.content != null && responseJson.message.content.Count > 0)
                        {
                            var text = responseJson.message.content[0].text.ToString();
                            return new AITextResponse()
                            {
                                ResponseMessage = text
                            };
                        }
                        else
                        {
                            // Return null if the response doesn't contain the expected content
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception("Error communicating with Cohere API.");
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt >= maxRetries)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                            "The request took longer than expected and timed out. Please try again in a moment.",
                            $"The request took longer than expected and timed out. Please try again in a moment.");
                    }

                    await Task.Delay(2000); // Delay for 2 seconds before retrying
                }
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "The request took longer than expected and timed out. Please try again in a moment.",
                $"The request took longer than expected and timed out. Please try again in a moment.");
        }

        #endregion MCQ Explination

        #region QNA

        public async Task<AITextResponse> GenerateQNA(TextRequestSM objSM)
        {
            if (objSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "No input data found", "No input data found");
            }

            var apiUrl = _cohereBaseUrl;
            var key = _cohereApiKey;

            var requestBody = new
            {
                model = _cohereSummerizeModel,
                messages = new[] {
                    new {
                        role = "user",
                        content = objSM.InputRequest
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");

            int maxRetries = 3;
            int attempt = 0;
            while (attempt < maxRetries)
            {

                try
                {
                    // Make the request
                    var response = await _httpClient.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        if (responseJson?.message?.content != null && responseJson.message.content.Count > 0)
                        {
                            var text = responseJson.message.content[0].text.ToString();
                            return new AITextResponse()
                            {
                                ResponseMessage = text
                            };
                        }
                        else
                        {
                            // Return null if the response doesn't contain the expected content
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception("Error communicating with Cohere API.");
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt >= maxRetries)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                            "The request took longer than expected and timed out. Please try again in a moment.",
                            $"The request took longer than expected and timed out. Please try again in a moment.");
                    }

                    await Task.Delay(2000); // Delay for 2 seconds before retrying
                }
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "The request took longer than expected and timed out. Please try again in a moment.",
                $"The request took longer than expected and timed out. Please try again in a moment.");
        }

        #endregion QNA
    }
}
