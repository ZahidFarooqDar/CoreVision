namespace CoreVisionConfig.Configuration
{
    public class ExternalIntegrations
    {
        public ExternalIntegrations()
        {
            this.StripeConfiguration = new StripeConfiguration();
            this.DataRaptorConfiguration = new DataRaptorConfiguration();
            this.HomeChefConfiguration = new HomeChefConfiguration();
            this.HuggingFaceConfiguration = new HuggingFaceConfiguration();
        }
        public DataRaptorConfiguration DataRaptorConfiguration { get; set; }
        public StripeConfiguration StripeConfiguration { get; set; }
        public HomeChefConfiguration HomeChefConfiguration { get; set; }
        public AzureConfiguration AzureConfiguration { get; set; }
        public HuggingFaceConfiguration HuggingFaceConfiguration { get; set; }

    }
    public class DataRaptorConfiguration
    {
        public string BaseUrl { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string ApiUserType { get; set; }
        public string CompanyCode { get; set; }
    }

    public class HomeChefConfiguration
    {
        public string BaseUrl { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string ApiUserType { get; set; }
        public string CompanyCode { get; set; }
    }


    public class StripeConfiguration
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string WHSecret { get; set; }
        public string MainApiProductKey { get; set; }
    }

    public class AzureConfiguration
    {

        public AzureConfiguration()
        {
            this.TextTranslatorConfiguration = new TextTranslatorConfiguration();
            this.TextAnalyticsConfiguration = new TextAnalyticsConfiguration();
            this.ComputerVisionConfiguration = new ComputerVisionConfiguration();
        }
        public ComputerVisionConfiguration ComputerVisionConfiguration { get; set; }
        public TextAnalyticsConfiguration TextAnalyticsConfiguration { get; set; }
        public TextTranslatorConfiguration TextTranslatorConfiguration { get; set; }
        public string SummafyAIModel { get; set; }
        public string AzureMapsApiKey { get; set; }
        public bool IsTestingMode { get; set; }
    }

    public class TextTranslatorConfiguration
    {
        public string ApiKey { get; set; }
        public string EndPoint { get; set; }
        public string DocumentTranslator { get; set; }
        public string Location { get; set; }
    }

    public class TextAnalyticsConfiguration
    {
        public string ApiKey { get; set; }
        public string EndPoint { get; set; }
    }

    public class ComputerVisionConfiguration
    {
        public string ApiKey { get; set; }
        public string EndPoint { get; set; }
        public string DocumentTranslator { get; set; }
        public string Location { get; set; }

    }
    public class HuggingFaceConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string TranscriptionModel { get; set; }
        public string SummarizeModel { get; set; }
        public string MinuteOfMeeting { get; set; }
        public string TranslationModel { get; set; }
        public string LanguageDetectionModel { get; set; }
        public string EntitiesDetectionModel { get; set; }
        public string ImageToTextModel { get; set; }
        public string TextToImageModel { get; set; }
        public string DeepSeekModel { get; set; }
        public string StoryAIModel { get; set; }
        public bool IsTestingMode { get; set; }
    }
}