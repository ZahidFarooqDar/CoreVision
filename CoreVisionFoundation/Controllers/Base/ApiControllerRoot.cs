using CoreVisionBAL.ExceptionHandler;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Linq;

namespace CoreVisionFoundation.Controllers.Base
{
    [ApiController]
    public abstract class ApiControllerRoot : ControllerBase
    {
        protected async Task<(ApiRequest<I>, IFormFileCollection)> TryReadApiRequestAsMultipart<I>(string[]? ValidContentTypes = null, string[]? ValidFileExtensions = null, long maxFileLength = 0L, bool ensureAtLeastOneFile = false) where I : class
        {
            if (string.IsNullOrWhiteSpace(HttpRequestMultipartExtensions.GetMultipartBoundary(base.Request)))
            {
                throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "MultiPart Boundry Not Found", "Uploaded file not found");
            }

            IFormCollection formCollection = await base.Request.ReadFormAsync();
            if (formCollection != null)
            {
                ApiRequest<I> requestData = null;
                if (formCollection?.Files != null && formCollection.Files.Count > 0)
                {
                    foreach (IFormFile file in formCollection.Files)
                    {
                        if (ValidFileExtensions != null && !ValidFileExtensions.Contains<string>(Path.GetExtension(file.FileName)))
                        {
                            throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "File: '$" + file.FileName + "' with extension is not valid");
                        }

                        if (ValidContentTypes != null && !ValidContentTypes.Contains<string>(Path.GetExtension(file.ContentType)))
                        {
                            throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "File type: '$" + file.ContentType + "' is not valid");
                        }

                        if (maxFileLength != 0L && file.Length > maxFileLength)
                        {
                            throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "File: '$" + file.FileName + "' too large.");
                        }
                    }
                }
                else if (ensureAtLeastOneFile)
                {
                    throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "At least one file is expected.");
                }

                KeyValuePair<string, StringValues> formApiItem = formCollection.FirstOrDefault<KeyValuePair<string, StringValues>>((KeyValuePair<string, StringValues> x) => x.Key == "apireq");
                if (!string.IsNullOrWhiteSpace(formApiItem.Value.FirstOrDefault()))
                {
                    requestData = JsonConvert.DeserializeObject<ApiRequest<I>>(formApiItem.Value.FirstOrDefault());
                    if (requestData == null)
                    {
                        throw new ApiExceptionRoot(ApiErrorTypeSM.InvalidInputData_Log, "ReqData invalid value: '" + formApiItem.Value.FirstOrDefault() + "'", "Request data is invalid.");
                    }
                }

                return (requestData, formCollection.Files);
            }

            return (null, null);
        }
    }
}
