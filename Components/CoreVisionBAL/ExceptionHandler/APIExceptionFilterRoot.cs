using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreVisionBAL.ExceptionHandler
{
    public abstract class APIExceptionFilterRoot : ExceptionFilterAttribute
    {
        public APIExceptionFilterRoot()
        {
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
        }
    }
}
