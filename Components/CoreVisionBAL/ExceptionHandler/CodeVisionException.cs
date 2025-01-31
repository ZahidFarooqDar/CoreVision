using CoreVisionServiceModels.Foundation.Base.Enums;
namespace CoreVisionBAL.ExceptionHandler
{
    public class CodeVisionException : ApiExceptionRoot
    {
        
        public CodeVisionException(ApiErrorTypeSM exceptionType, string devMessage,
           string displayMessage = "", Exception innerException = null)
            : base(exceptionType, devMessage, displayMessage, innerException)
        { }
    }
}
