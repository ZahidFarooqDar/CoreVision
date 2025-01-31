﻿using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CoreVisionBAL.ExceptionHandler
{
    public partial class APIExceptionFilter : APIExceptionFilterRoot
    {
        private readonly ErrorLogHandlerRoot _errorLogHandlerRoot;

        public APIExceptionFilter(ErrorLogHandlerRoot errorLogHandlerRoot)
        {
            _errorLogHandlerRoot = errorLogHandlerRoot;
        }
        public override void OnException(ExceptionContext context)
        {

            ErrorData errorData = _errorLogHandlerRoot.HandleException(context.Exception, context.HttpContext);
            if (errorData == null)
            {
                errorData = new ErrorData() { DisplayMessage = "Unknown Error, Could Not Log.", ApiErrorType = ApiErrorTypeSM.FrameworkException_Log };
            }

            var errResp = new ApiResponse<object>()
            {
                IsError = true,
                ErrorData = errorData
            };

            context.Result = new ObjectResult(errResp)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            base.OnException(context);
        }
    }
}
