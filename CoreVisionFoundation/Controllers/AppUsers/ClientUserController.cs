using CoreVisionBAL.AppUsers;
using CoreVisionConfig.Configuration;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.AppUser;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General;
using Microsoft.AspNetCore.Authorization;
using CoreVisionFoundation.Controllers.Base;
using Microsoft.AspNetCore.OData.Query;
using CoreVisionBAL.Foundation.Web;
using Microsoft.AspNetCore.Mvc;

namespace CoreVisionFoundation.Controllers.AppUsers
{
    [ApiController]
    [Route("api/v1/[controller]")]   
   
    public partial class ClientUserController : ApiControllerWithOdataRoot<ClientUserSM>
    {
        #region Properties
        private readonly ClientUserProcess _clientUserProcess;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        #endregion Properties

        #region Constructor
        public ClientUserController(ClientUserProcess process, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
            : base(process)
        {
            _clientUserProcess = process;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion Constructor

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientUserSM>>>> GetAsOdata(ODataQueryOptions<ClientUserSM> oDataOptions)
        {
            //oDataOptions.Filter = new FilterQueryOption();
            //TODO: validate inputs here probably 
            //if (oDataOptions.Filter == null)
            //    oDataOptions.Filter. = "$filter=organisationUnitId%20eq%20" + 10 + ",";
            var retList = await GetAsEntitiesOdata(oDataOptions);

            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata EndPoints

        #region Get Endpoints

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientUserSM>>>> GetAll()
        {
            var listSM = await _clientUserProcess.GetAllClientUsers();
            return Ok(ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("company/{companyId}/{skip}/{top}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientUserSM>>>> GetUsersByCompanyId(int companyId, int skip, int top)
        {
            var listSM = await _clientUserProcess.GetUsersByCompanyId(companyId, skip, top);
            return Ok(ModelConverter.FormNewSuccessResponse(listSM));
        }

        [HttpGet("companyusers/count/{companyId}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetCountOfCompanyUsers(int companyId)
        {
            var countResp = await _clientUserProcess.GetCountOfCompanyUsers(companyId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(countResp, "Total Count of Company Users")));
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> GetById(int id)
        {
            var singleSM = await _clientUserProcess.GetClientUserById(id);
            if (singleSM != null)
            {
                return ModelConverter.FormNewSuccessResponse(singleSM);
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        [HttpGet("mine")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> GetMine()
        {
            var id = User.GetUserRecordIdFromCurrentUserClaims();
            var singleSM = await _clientUserProcess.GetClientUserById(id);
            if (singleSM != null)
            {
                return ModelConverter.FormNewSuccessResponse(singleSM);
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }


        #endregion Get Endpoints

        #region Add/Update Endpoints

        [HttpPost()]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> PostNewAppUser([FromBody] ApiRequest<ClientUserSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var companyCode = "123";
            #endregion Check Request

            var addedSM = await _clientUserProcess.AddNewUser(innerReq, companyCode);
            if (addedSM != null)
            {
                return ModelConverter.FormNewSuccessResponse(addedSM);
            }
            else
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        [HttpPut("mine")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> Put([FromBody] ApiRequest<ClientUserSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var id = User.GetUserRecordIdFromCurrentUserClaims();
             if (id <= 0)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            #endregion Check Request

            var updatedSM = await _clientUserProcess.UpdateClientUser(id, innerReq);
            if (updatedSM != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(updatedSM));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }


        [HttpGet("dashboard")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> Dashboard()
        {
            #region Check Request

            var id = User.GetUserRecordIdFromCurrentUserClaims();
            if (id <= 0)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdInvalid, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            #endregion Check Request

            var updatedSM = await _clientUserProcess.GetUserDashboard(id);
            if (updatedSM != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(updatedSM));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        #endregion Add/Update Endpoints

        #region Check Email/loginId and Verify Email

        #region Check Existing Email/LoginId

        [HttpGet("check/email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> CheckEmail(string email)
        {
            var companyCode = "123";
            var resp = await _clientUserProcess.CheckExistingEmail(email, companyCode);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        [HttpGet("check/loginId")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> CheckLoginId(string loginId)
        {
            var companyCode = "123";
            var resp = await _clientUserProcess.CheckExistingLoginId(loginId, companyCode);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        #endregion Check Existing Email/loginId 

        #region Verify Email

        [HttpPost("VerifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> VerfiyEmail([FromBody] ApiRequest<VerifyEmailOTPRequestSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }


            #endregion Check Request
            var resp = await _clientUserProcess.ConfirmEmailOtpAsync(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        [HttpPost("resend-email-otp")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ResendEmailVerification([FromBody] ApiRequest<EmailConfirmationSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }


            #endregion Check Request
            var resp = await _clientUserProcess.ResendEmailOTPVerification(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        [HttpPost("forgotloginid")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ForgotLoginId([FromBody] ApiRequest<EmailConfirmationSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }


            #endregion Check Request
            var resp = await _clientUserProcess.SendLoginIdToEmail(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        #endregion Verify Email

        #endregion Check Email/loginId and Verify Email

        #region Delete Endpoints

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(int id)
        {
            var resp = await _clientUserProcess.DeleteClientUserById(id);
            if (resp != null && resp.DeleteResult)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(resp?.DeleteMessage, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        [HttpDelete("mine/logo")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> DeleteUserProfilePicture()
        {
            #region Check Request

            int currentCompanyId = User.GetCompanyRecordIdFromCurrentUserClaims();
            if (currentCompanyId <= 0)
            { return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotInClaims)); }

            #endregion Check Request

            var resp = await _clientUserProcess.DeleteProfilePictureById(currentCompanyId, _environment.WebRootPath);
            if (resp != null && resp.DeleteResult)
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            else
                return NotFound(ModelConverter.FormNewErrorResponse(resp?.DeleteMessage, ApiErrorTypeSM.NoRecord_NoLog));
        }

        #endregion Delete Endpoints

        #region ForgotPassword & ResetPassword EndPoints

        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> ForgotPassword([FromBody] ApiRequest<ForgotPasswordSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.ToUriComponent()}";
            var link = $"{baseUrl}/resetpassword";
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            #endregion Check Request

            var resp = await _clientUserProcess.SendResetPasswordLink(innerReq, link);
            if (resp != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ClientUserSM>>> ResetPassword([FromBody] ApiRequest<ResetPasswordRequestSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            var resp = await _clientUserProcess.UpdatePassword(innerReq);
            if (resp != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        [HttpGet("validatepassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> ValidatePassword(string authCode)
        {
            var resp = await _clientUserProcess.ValidatePassword(authCode);
            if (resp != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        #endregion ForgotPassword & ResetPassword EndPoints

        #region Set/Update Password
               

        [HttpPost("SetPassword")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee,ClientAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> SetPassword([FromBody] ApiRequest<SetPasswordRequestSM> apiRequest)
        {
            int currentUserRecordId = User.GetUserRecordIdFromCurrentUserClaims();
            if (currentUserRecordId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotInClaims));
            }

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var resp = await _clientUserProcess.SetPassword(currentUserRecordId, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        [HttpPost("UpdatePassword")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee,ClientAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> ChangePassword([FromBody] ApiRequest<UpdatePasswordRequestSM> apiRequest)
        {
            int currentUserRecordId = User.GetUserRecordIdFromCurrentUserClaims();
            if (currentUserRecordId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotInClaims));
            }

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            var resp = await _clientUserProcess.ChangePassword(currentUserRecordId, innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(resp));
        }

        #endregion Set/Update Password
    }
}
