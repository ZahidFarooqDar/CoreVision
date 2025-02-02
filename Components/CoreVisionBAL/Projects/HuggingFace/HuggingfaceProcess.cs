using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.HuggingFace;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using CoreVisionServiceModels.Enums;

namespace CoreVisionBAL.Projects.HuggingFace
{    
    public class HuggingfaceProcess : CoreVisionBalBase
    {
        #region Properties
        private readonly APIConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly int _port;
        private readonly string _baseUrl;
        private readonly string _apiKey;
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

        #endregion Properties

        #region Constructor
        
        public HuggingfaceProcess(IMapper mapper, ApiDbContext apiDbContext, APIConfiguration configuration, HttpClient httpClient)
        : base(mapper, apiDbContext)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _baseUrl = configuration.ExternalIntegrations.HuggingFaceConfiguration.BaseUrl;
            _apiKey = configuration.ExternalIntegrations.HuggingFaceConfiguration.ApiKey;
            _transcriptionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TranscriptionModel;
            //_httpClient.DefaultRequestHeaders.Add($"Authorization", $"Bearer {_apiKey}"); //Hugging Face            
            _summarizeModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.SummarizeModel;
            _minuteOfMeting = configuration.ExternalIntegrations.HuggingFaceConfiguration.MinuteOfMeeting;
            _translationModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TranslationModel;
            _languageDetectionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.LanguageDetectionModel;
            _entitiesDetectionModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.EntitiesDetectionModel;
            _imageToTextModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.ImageToTextModel;
            _textToImageModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.TextToImageModel;
            _deepSeekModel = configuration.ExternalIntegrations.HuggingFaceConfiguration.DeepSeekModel;
            _isHuggingFaceTestingMode = configuration.ExternalIntegrations.HuggingFaceConfiguration.IsTestingMode;
        }

        #endregion Constructor

        #region Transcribe Audio
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="CoreVisionException"></exception>
        public async Task<AudioTranscriptionResponseSM> TranscribeAudioUsingHuggingFaceAsync(AudioTranscriptionRequestSM request)
        {

            if (_isHuggingFaceTestingMode)
            {
                return GetDummyResponse(); // Return dummy response for testing mode
            }

            if (string.IsNullOrEmpty(request.AudioBase64))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "No audio data provided.");
            }
            byte[] audioBytes = Convert.FromBase64String(request.AudioBase64);

            var audioStream = new MemoryStream(audioBytes);

            string contentType = request.Extension.ToLower() switch
            {
                ".wav" => "audio/wav",
                ".mp3" => "audio/mpeg",
                ".flac" => "audio/flac",
                _ => throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Unsupported file type: " + request.Extension),
            };

            var requestContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(audioStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            requestContent.Add(fileContent, "file", "audioFile" + request.Extension); // Name the file dynamically

            requestContent.Add(new StringContent("true"), "return_timestamps");
            requestContent.Add(new StringContent("en"), "target_lang");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);

            var retryCount = 3;
            var delay = 2000; // Delay between retries in milliseconds

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    var response = await _httpClient.PostAsync(_baseUrl + _transcriptionModel, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                        var transcription = jsonResponse?.text;
                        var res = new AudioTranscriptionResponseSM()
                        {
                            Response = transcription?.ToString()
                        };
                        return res;
                    }
                    else
                    {
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error: {response.StatusCode}, Details: {errorDetails}");
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");

                    if (i == retryCount - 1)
                    {
                        throw new CoreVisionException(
                            ApiErrorTypeSM.Fatal_Log,
                            $"Failed after {retryCount} attempts",
                            $"Failed after {retryCount} attempts"
                        );
                    }

                    // Wait for some time before retrying
                    await Task.Delay(delay);
                }
            }

            return new AudioTranscriptionResponseSM();
        }

        #endregion Transcribe Audio

        #region Image to Text

        public async Task<HuggingFaceResponseSM> ExtractTextFromImageAsync(ImageDataSM request)
        {
            if (_isHuggingFaceTestingMode)
            {
                return GetDummySummafyAIResponse();
            }
            var model = _imageToTextModel;
            int maxRetries = 3;
            int attempts = 0;
            string result = null;
            string responseString = null;

            while (attempts < maxRetries)
            {
                try
                {
                    var requestData = new
                    {
                        inputs = request.Base64Image// Base64 encoded image string
                    };

                    result = await MakeHuggingFaceRequest(model, request.Base64Image);

                    if (string.IsNullOrEmpty(result))
                    {
                        return null;
                    }

                    var extractedTextList = JsonConvert.DeserializeObject<List<dynamic>>(result);
                    var extractedTextChunks = string.Join(" ", extractedTextList.Select(m => m.generated_text));
                    responseString = extractedTextChunks;

                    var res = new HuggingFaceResponseSM()
                    {
                        TextResponse = responseString,
                    };
                    return res;
                }
                catch (Exception ex)
                {
                    attempts++;

                    if (attempts >= maxRetries)
                    {
                        // If max retries are reached, throw the exception
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"OCR extraction failed after {maxRetries} attempts", $"{ex.Message}");
                    }

                    // Wait before retrying
                    await Task.Delay(2000); // 2-second delay before retry
                }
            }

            // If all attempts fail, throw an exception
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"OCR extraction failed after {maxRetries} attempts", $"OCR extraction failed after {maxRetries} attempts");
        }


        #endregion Image to Text

        #region Hugging Face Request        
        public async Task<string> MakeHuggingFaceRequest(string model, string inputText)
        {

            var requestBody = new
            {
                inputs = inputText
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync($"{_baseUrl}{model}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorDetails);

                // Fetch the error message
                var errorMessage = errorResponse.ContainsKey("error") ? errorResponse["error"] : string.Empty;
                var specificErrorMessage = errorMessage.Contains("Model not found") ? "Model not found" : string.Empty;
                string specificMessage = "";
                if (!specificErrorMessage.IsNullOrEmpty())
                {
                    specificMessage = "Translation unavailable for this language. Try another.";
                }
                if (specificMessage.IsNullOrEmpty())
                {
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    $"Something went wrong. Please retry or contact support.",
                    $"Something went wrong while fetching data. Status Code: {errorMessage}.Request failed with status code {response.StatusCode}"
                    );
                }
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    $"{specificMessage}",
                    $"Something went wrong while fetching data. Status Code: {errorMessage}.Request failed with status code {response.StatusCode}"
                    );
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        #endregion Hugging Face Request

        #region DeepSeek QNA

        public async Task<HuggingFaceResponseSM> ExtractResponseUsingDeepSeekAsync(TextRequestSM request)
        {
            if (_isHuggingFaceTestingMode)
            {
                return GetDummySummafyAIResponse();
            }
            int maxRetries = 3;
            int attempts = 0;
            string result = null;
            string responseString = null;

            while (attempts < maxRetries)
            {
                try
                {
                    result = await MakeHuggingFaceRequest(_deepSeekModel, request.InputRequest);

                    if (string.IsNullOrEmpty(result))
                    {
                        return null;
                    }

                    var extractedTextList = JsonConvert.DeserializeObject<List<dynamic>>(result);
                    var extractedTextChunks = string.Join(" ", extractedTextList.Select(m => m.generated_text));
                    responseString = extractedTextChunks;

                    return new HuggingFaceResponseSM()
                    {
                        TextResponse = responseString,
                    };
                }
                catch (Exception ex)
                {
                    attempts++;

                    if (attempts >= maxRetries)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                            $"DeepSeek extraction failed after {maxRetries} attempts",
                            $"{ex.Message}");
                    }

                    await Task.Delay(2000); // Retry after delay
                }
            }

            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                $"DeepSeek extraction failed after {maxRetries} attempts",
                $"DeepSeek extraction failed after {maxRetries} attempts");
        }


        #endregion DeepSeek QNA

        #region Translate Text

        public async Task<HuggingFaceResponseSM> TranslateTextAsync(TranslationRequestSM request, int maxRetries = 3)
        {
            
            // Define the maximum allowed input length for the model
            const int maxTokenLength = 512;

            // Split the input text into chunks that fit within the token limit
            var chunks = SplitIntoChunks(request.Text, maxTokenLength);

            // Detect the source language using the first chunk
            var firstChunk = chunks.FirstOrDefault();
            if (string.IsNullOrEmpty(firstChunk))
            {
                throw new CoreVisionException(
                            ApiErrorTypeSM.Fatal_Log,
                            "Input text is empty or invalid.",
                            $"Input text is empty or invalid."
                        );
            }

            var firstFiveWords = string.Join(" ", firstChunk.Split(' ').Take(5));
            var fromLanguage = await DetectLanguageAsync(firstFiveWords);
            var targetLanguage = GetTranslationLanguageCodeFromName(request.Language);
            // Construct the translation model identifier
            var model = $"{_translationModel}{fromLanguage}-{targetLanguage}";

            var translatedText = new List<string>();

            foreach (var chunk in chunks)
            {
                int attempt = 0;
                while (attempt < maxRetries)
                {
                    attempt++;

                    try
                    {
                        // Attempt to translate the current chunk
                        var result = await MakeHuggingFaceRequest(model, chunk);
                        var translatedTextList = JsonConvert.DeserializeObject<List<dynamic>>(result);
                        var translatedTextChunks = string.Join(" ", translatedTextList.Select(m => m.translation_text));
                        translatedText.Add(translatedTextChunks);
                        break; // Break out of the retry loop for this chunk
                    }
                    catch (Exception ex)
                    {
                        if (attempt == maxRetries)
                        {
                            // Log or handle the error and add a placeholder for the failed chunk
                            // translatedText.Add($"[Error: {ex.Message}]");
                            var msg = ex.Message;
                            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"{msg}", $"{msg}");

                        }
                        else
                        {
                            // Wait for a short delay before retrying
                            await Task.Delay(2000);
                        }
                    }
                }
            }
            var translationResponse = string.Join(" ", translatedText);
            if (translationResponse.IsNullOrEmpty())
            {
                //return null;
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Oops! Something went wrong while translating your text. Please check your input and try again.", "Oops! Something went wrong while translating your text. Please check your input and try again.");
            }
            var res = new HuggingFaceResponseSM()
            {
                TextResponse = translationResponse,
            };
            return res;
        }

        private IEnumerable<string> SplitIntoChunks(string text, int maxChunkSize)
        {
            var words = text.Split(' ');
            var currentChunk = new List<string>();
            var currentSize = 0;

            foreach (var word in words)
            {
                if (currentSize + word.Length + 1 > maxChunkSize)
                {
                    yield return string.Join(" ", currentChunk);
                    currentChunk.Clear();
                    currentSize = 0;
                }

                currentChunk.Add(word);
                currentSize += word.Length + 1; // Account for space between words
            }

            if (currentChunk.Count > 0)
            {
                yield return string.Join(" ", currentChunk);
            }
        }
        private async Task<string> DetectLanguageAsync(string inputText, int maxRetries = 3)
        {

            var model = _languageDetectionModel; // Use a language detection model on Hugging Face or another service

            int attempt = 0;

            while (attempt < maxRetries)
            {
                attempt++;

                try
                {
                    var result = await MakeHuggingFaceRequest(model, inputText);

                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                    var topLanguage = jsonResponse[0];

                    string languageCode = topLanguage[0]?.label;

                    if (languageCode != null)
                    {
                        var language = languageCode;
                        return language;
                    }

                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Oops! We couldn't detect the language. Please try again.", "Oops! We couldn't detect the language. Please try again.");
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Oops! We couldn't detect the language.Error {ex.Message}, Please try again.", $"Oops! We couldn't detect the language.Error {ex.Message}, Please try again.");
                        // If the maximum retry attempts are reached, return the error message
                        //return $"Error: {ex.Message}";
                    }
                    else
                    {
                        // Optionally log the exception or handle retries here
                        // Delay for 2 seconds before retrying (optional, adjust as needed)
                        await Task.Delay(2000); // Delay for 2 seconds before retrying
                    }
                }
            }

            // If all retries fail, return a failure message
            //return "Error: Maximum retry attempts reached.";
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Oops! We couldn't detect the language, Please try again.", $"Oops! We couldn't detect the language, Please try again.");
        }



        #endregion Translate Text

        #region Summerize text        
        public async Task<AudioTranscriptionResponseSM> SummarizeTextAsync(HuggingFaceRequestSM request)
        {
            if (_isHuggingFaceTestingMode)
            {
                return GetDummyResponse();
            }

            var model = _summarizeModel;
            int maxRetries = 3;
            int attempts = 0;
            string result = null;
            var summerizedText = new List<string>();
            string responseString = null;

            var inputChunks = SplitTextIntoChunks(request.InputRequest, 3500);

            while (attempts < maxRetries)
            {
                try
                {
                    foreach (var chunk in inputChunks)
                    {
                        result = await MakeHuggingFaceRequest(model, chunk);

                        if (result.IsNullOrEmpty())
                        {
                            return null;
                        }

                        var summarizedTextList = JsonConvert.DeserializeObject<List<dynamic>>(result);
                        var summerizedTextChunks = string.Join(" ", summarizedTextList.Select(m => m.summary_text));
                        summerizedText.Add(summerizedTextChunks);
                    }

                    if (summerizedText.Count > 0)
                    {
                        responseString = string.Join(" ", summerizedText);
                    }

                    var res = new AudioTranscriptionResponseSM()
                    {
                        Response = responseString,
                    };
                    return res;
                }
                catch (Exception ex)
                {
                    attempts++;

                    if (attempts >= maxRetries)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Summarization failed after {maxRetries} attempts", $"{ex.Message}");
                    }

                    await Task.Delay(2000); // 2-second delay before retry
                }
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Summarization failed after {maxRetries} attempts", $"Summarization failed after {maxRetries} attempts");
        }

        private List<string> SplitTextIntoChunks(string text, int maxChunkSize)
        {
            var chunks = new List<string>();
            for (int i = 0; i < text.Length; i += maxChunkSize)
            {
                chunks.Add(text.Substring(i, Math.Min(maxChunkSize, text.Length - i)));
            }
            return chunks;
        }
        #endregion Summerize text

        #region Image Generation

        public async Task<Base64ImageResponseSM> GenerateHuggingImageAsync(HuggingFaceRequestSM request)
        {
            var requestContent = new
            {
                inputs = request.InputRequest
            };

            var jsonContent = JsonConvert.SerializeObject(requestContent);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Retry mechanism for image generation API calls
            int maxRetries = 3;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    // Make a POST request to generate the image
                    //var response = await MakeHuggingFaceRequest(_textToImageModel, prompt);
                    var response = await _httpClient.PostAsync($"{_baseUrl}{_textToImageModel}", httpContent); ;

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                        var base64 = Convert.ToBase64String(imageBytes);
                        return new Base64ImageResponseSM()
                        {
                            Base64Image = base64
                        };

                    }
                    else
                    {
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                            $"{errorDetails}",
                            $"Something went wrong while fetching data. Status Code: {errorDetails}.Request failed with status code {response.StatusCode}"
                    );
                    }
                }
                catch (Exception ex)
                {
                    // Log error and retry if retries are remaining
                    //_logger.LogError($"Failed to generate image for prompt '{prompt}': {ex.Message}. Retry {retry + 1}/{maxRetries}.");

                    if (retry == maxRetries - 1) throw; // Fail after max retries
                }
            }

            return null;
        }

        #endregion Image Generation

        #region Dummy Response

        public AudioTranscriptionResponseSM GetDummyResponse()
        {
            var res = new AudioTranscriptionResponseSM()
            {
                Response = "This is a dummy response for your input, designed to facilitate the integration of text extraction, summarization, and translation functionalities. " +
                               "It serves as a placeholder while the actual Hugging Face AI integration is being tested, ensuring the system functions smoothly. " +
                               "This response will allow you to test the flow and connectivity of the integration without depending on real API calls or data."
            };
            return res;
        }
        public HuggingFaceResponseSM GetDummySummafyAIResponse()
        {
            var res = new HuggingFaceResponseSM()
            {
                TextResponse = "This is a dummy response for your input, designed to facilitate the integration of text extraction, summarization, and translation functionalities. " +
                               "It serves as a placeholder while the actual Azure AI integration is being tested, ensuring the system functions smoothly. " +
                               "This response will allow you to test the flow and connectivity of the integration without depending on real API calls or data."
            };
            return res;
        }

        public string GetTranslationLanguageCodeFromName(TextTranslationLanguageSupportSM languageName)
        {

            foreach (var pair in TranslationLanguageMapping)
            {
                if (pair.Value == languageName)
                {
                    return pair.Key;
                }
            }
            return "unk";
        }

        #region Text Translation Language Mapper (Dictionary)


        private static readonly Dictionary<string, TextTranslationLanguageSupportSM> TranslationLanguageMapping = new()
        {
            { "af",         TextTranslationLanguageSupportSM.Afrikaans},
            { "am",         TextTranslationLanguageSupportSM.Amharic},
            { "ar",         TextTranslationLanguageSupportSM.Arabic },
            { "as",         TextTranslationLanguageSupportSM.Assamese },
            { "az",         TextTranslationLanguageSupportSM.Azerbaijani},
            { "ba",         TextTranslationLanguageSupportSM.Bashkir },
            { "bg",         TextTranslationLanguageSupportSM.Bulgarian },
            { "bh",         TextTranslationLanguageSupportSM.Bhojpuri },
            { "bn",         TextTranslationLanguageSupportSM.Bangla},
            { "bo",         TextTranslationLanguageSupportSM.Tibetan },
            { "brx",        TextTranslationLanguageSupportSM.Bodo },
            { "bs",         TextTranslationLanguageSupportSM.Bosnian },
            { "ca",         TextTranslationLanguageSupportSM.Catalan },
            { "cs",         TextTranslationLanguageSupportSM.Czech },
            { "cy",         TextTranslationLanguageSupportSM.Welsh },
            { "da",         TextTranslationLanguageSupportSM.Danish },
            { "de",         TextTranslationLanguageSupportSM.German },
            { "doi",        TextTranslationLanguageSupportSM.Dogri },
            { "dsb",        TextTranslationLanguageSupportSM.LowerSorbian },
            { "dv",         TextTranslationLanguageSupportSM.Divehi },
            { "el",         TextTranslationLanguageSupportSM.Greek },
            { "en",         TextTranslationLanguageSupportSM.English },
            { "es",         TextTranslationLanguageSupportSM.Spanish },
            { "et",         TextTranslationLanguageSupportSM.Estonian },
            { "eu",         TextTranslationLanguageSupportSM.Basque },
            { "fa",         TextTranslationLanguageSupportSM.Persian },
            { "fi",         TextTranslationLanguageSupportSM.Finnish },
            { "fil",        TextTranslationLanguageSupportSM.Filipino },
            { "fj",         TextTranslationLanguageSupportSM.Fijian },
            { "fo",         TextTranslationLanguageSupportSM.Faroese },
            { "fr",         TextTranslationLanguageSupportSM.French },
            { "fr-CA",      TextTranslationLanguageSupportSM.French_Canada },
            { "ga",         TextTranslationLanguageSupportSM.Irish },
            { "gl",         TextTranslationLanguageSupportSM.Galician },
            { "kok",        TextTranslationLanguageSupportSM.Konkani },
            { "gu",         TextTranslationLanguageSupportSM.Gujarati },
            { "ha",         TextTranslationLanguageSupportSM.Hausa },
            { "he",         TextTranslationLanguageSupportSM.Hebrew },
            { "hi",         TextTranslationLanguageSupportSM.Hindi },
            { "hne",        TextTranslationLanguageSupportSM.Chhattisgarhi },
            { "hr",         TextTranslationLanguageSupportSM.Croatian },
            { "hsb",        TextTranslationLanguageSupportSM.UpperSorbian },
            { "ht",         TextTranslationLanguageSupportSM.HaitianCreole },
            { "hu",         TextTranslationLanguageSupportSM.Hungarian },
            { "hy",         TextTranslationLanguageSupportSM.Armenian },
            { "id",         TextTranslationLanguageSupportSM.Indonesian },
            { "ig",         TextTranslationLanguageSupportSM.Igbo },
            { "ikt",        TextTranslationLanguageSupportSM.Inuinnaqtun },
            { "is",         TextTranslationLanguageSupportSM.Icelandic },
            { "it",         TextTranslationLanguageSupportSM.Italian },
            { "iu",         TextTranslationLanguageSupportSM.Inuktitut },
            { "iu-Latn",    TextTranslationLanguageSupportSM.Inuktitut_Latin },
            { "ja",         TextTranslationLanguageSupportSM.Japanese },
            { "ka",         TextTranslationLanguageSupportSM.Georgian },
            { "kk",         TextTranslationLanguageSupportSM.Kazakh },
            { "km",         TextTranslationLanguageSupportSM.Khmer },
            { "kmr",        TextTranslationLanguageSupportSM.Kurdish_Northern },
            { "kn",         TextTranslationLanguageSupportSM.Kannada },
            { "ko",         TextTranslationLanguageSupportSM.Korean },
            { "ks",         TextTranslationLanguageSupportSM.Kashmiri },
            { "ku",         TextTranslationLanguageSupportSM.Kurdish_Central },
            { "ky",         TextTranslationLanguageSupportSM.Kyrgyz },
            { "ln",         TextTranslationLanguageSupportSM.Lingala },
            { "lo",         TextTranslationLanguageSupportSM.Lao },
            { "lt",         TextTranslationLanguageSupportSM.Lithuanian },
            { "lg",         TextTranslationLanguageSupportSM.Ganda },
            { "lv",         TextTranslationLanguageSupportSM.Latvian },
            { "lzh",        TextTranslationLanguageSupportSM.Chinese_Literary },
            { "mai",        TextTranslationLanguageSupportSM.Maithili },
            { "mg",         TextTranslationLanguageSupportSM.Malagasy },
            { "mi",         TextTranslationLanguageSupportSM.Maori },
            { "mk",         TextTranslationLanguageSupportSM.Macedonian },
            { "ml",         TextTranslationLanguageSupportSM.Malayalam },
            { "mn",         TextTranslationLanguageSupportSM.Mongolian_Cyrillic },
            { "mn-Mong",    TextTranslationLanguageSupportSM.Mongolian_Traditional },
            { "mni",        TextTranslationLanguageSupportSM.Manipuri },
            { "mr",         TextTranslationLanguageSupportSM.Marathi },
            { "ms",         TextTranslationLanguageSupportSM.Malay},
            { "mt",         TextTranslationLanguageSupportSM.Maltese },
            { "mww",        TextTranslationLanguageSupportSM.HmongDaw },
            { "my",         TextTranslationLanguageSupportSM.Myanmar_Burmese},
            { "nb",         TextTranslationLanguageSupportSM.Norwegian },
            { "ne",         TextTranslationLanguageSupportSM.Nepali },
            { "nl",         TextTranslationLanguageSupportSM.Dutch },
            { "nso",        TextTranslationLanguageSupportSM.Sesotho_sa_Leboa },
            { "ny",         TextTranslationLanguageSupportSM.Nyanja },
            { "or",         TextTranslationLanguageSupportSM.Odia },
            { "otq",        TextTranslationLanguageSupportSM.QueretaroOtomi },
            { "pa",         TextTranslationLanguageSupportSM.Punjabi },
            { "pl",         TextTranslationLanguageSupportSM.Polish },
            { "prs",        TextTranslationLanguageSupportSM.Dari },
            { "ps",         TextTranslationLanguageSupportSM.Pashto },
            { "pt-BR",      TextTranslationLanguageSupportSM.Portuguese_Brazil },
            { "pt-PT",      TextTranslationLanguageSupportSM.Portuguese_Brazil },
            { "ro",         TextTranslationLanguageSupportSM.Romanian },
            { "ru",         TextTranslationLanguageSupportSM.Russian },
            { "rn",         TextTranslationLanguageSupportSM.Rundi },
            { "rw",         TextTranslationLanguageSupportSM.Kinyarwanda },
            { "sd",         TextTranslationLanguageSupportSM.Sindhi },
            { "si",         TextTranslationLanguageSupportSM.Sinhala },
            { "sk",         TextTranslationLanguageSupportSM.Slovak },
            { "sl",         TextTranslationLanguageSupportSM.Slovenian },
            { "sm",         TextTranslationLanguageSupportSM.Samoan },
            { "sn",         TextTranslationLanguageSupportSM.Shona },
            { "so",         TextTranslationLanguageSupportSM.Somali },
            { "sq",         TextTranslationLanguageSupportSM.Albanian },
            { "sr-Cyrl",    TextTranslationLanguageSupportSM.Serbian_Cyrillic },
            { "sr-Latn",    TextTranslationLanguageSupportSM.Serbian_Latin },
            { "st",         TextTranslationLanguageSupportSM.Sesotho },
            { "sv",         TextTranslationLanguageSupportSM.Swedish },
            { "sw",         TextTranslationLanguageSupportSM.Swahili },
            { "ta",         TextTranslationLanguageSupportSM.Tamil },
            { "te",         TextTranslationLanguageSupportSM.Telugu },
            { "th",         TextTranslationLanguageSupportSM.Thai },
            { "ti",         TextTranslationLanguageSupportSM.Tigrinya },
            { "tk",         TextTranslationLanguageSupportSM.Turkmen },
            { "tlh-Latn",   TextTranslationLanguageSupportSM.Klingon_Latin },
            { "tlh-pIqaD",  TextTranslationLanguageSupportSM.Klingon_pIqaD },
            { "tn",         TextTranslationLanguageSupportSM.Setswana },
            { "to",         TextTranslationLanguageSupportSM.Tongan },
            { "tr",         TextTranslationLanguageSupportSM.Turkish },
            { "tt",         TextTranslationLanguageSupportSM.Tatar },
            { "ty",         TextTranslationLanguageSupportSM.Tahitian },
            { "ug",         TextTranslationLanguageSupportSM.Uyghur },
            { "uk",         TextTranslationLanguageSupportSM.Ukrainian },
            { "ur",         TextTranslationLanguageSupportSM.Urdu },
            { "uz",         TextTranslationLanguageSupportSM.Uzbek_Latin },
            { "vi",         TextTranslationLanguageSupportSM.Vietnamese }
        };

        #endregion Text Translation Language Mapper (Dictionary)

        #endregion Dummy Response
    }
}
