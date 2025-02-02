using CoreVisionServiceModels.Foundation.Base.Enums;
namespace CoreVisionBAL.ExceptionHandler
{
    public class CoreVisionException : ApiExceptionRoot
    {
        
        public CoreVisionException(ApiErrorTypeSM exceptionType, string devMessage,
           string displayMessage = "", Exception innerException = null)
            : base(exceptionType, devMessage, displayMessage, innerException)
        { }
    }
}
