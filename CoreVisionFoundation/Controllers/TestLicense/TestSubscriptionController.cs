using CoreVisionBAL.Foundation.Web;
using CoreVisionBAL.License;
using CoreVisionFoundation.Controllers.Base;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.License;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CoreVisionFoundation.Controllers.TestLicense
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestSubscriptionController : ApiControllerWithOdataRoot<UserTestLicenseDetailsSM>
    {
        private readonly UserTestLicenseDetailsProcess _testSubscriptionProcess;
        public TestSubscriptionController(UserTestLicenseDetailsProcess process)
            : base(process)
        {
            _testSubscriptionProcess = process;
        }

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestLicenseDetailsSM>>>> GetAsOdata(ODataQueryOptions<UserTestLicenseDetailsSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata

        #region Get All/By Id and Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestLicenseDetailsSM>>>> GetAll()
        {
            var retList = await _testSubscriptionProcess.GetAllTestUserLicenseDetails();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("mine")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestLicenseDetailsSM>>>> GetAllMineLicenses()
        {
            #region Check Request

            var userId = User.GetUserRecordIdFromCurrentUserClaims();

            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound));
            }

            #endregion Check Request

            var retList = await _testSubscriptionProcess.GetTestUserSubscriptionByUserId(userId);

            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("mine/active")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<UserTestLicenseDetailsSM>>> GetMineActiveLicense()
        {
            #region Check Request

            var userId = User.GetUserRecordIdFromCurrentUserClaims();

            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound));
            }

            #endregion Check Request

            var retList = await _testSubscriptionProcess.GetActiveUserLicenseDetailsByUserId(userId);

            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestLicenseDetailsSM>>> GetById(int id)
        {
            var ret = await _testSubscriptionProcess.GetUserSubscriptionById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllLicensesCount()
        {
            var ret = await _testSubscriptionProcess.GetAllTestUserLicenseDetailsCount();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total License Types")));
        }

        [HttpGet("mine/count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllMineLicensesCount()
        {

            #region Check Request

            var userId = User.GetUserRecordIdFromCurrentUserClaims();

            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound));
            }

            #endregion Check Request

            var ret = await _testSubscriptionProcess.GetActiveUserLicenseTestCountsByUserId(userId);
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total License Types")));
        }

        #endregion Get

        #region Add/Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestLicenseDetailsSM>>> Add(int testLicenseId, string username, [FromBody] ApiRequest<UserTestLicenseDetailsSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _testSubscriptionProcess.AddTestUserSubscription(innerReq, testLicenseId, username);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestLicenseDetailsSM>>> Update(int id, [FromBody] ApiRequest<UserTestLicenseDetailsSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _testSubscriptionProcess.UpdateUserSubscription(id,innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("status/{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestLicenseDetailsSM>>> UpdateLicenseStatus(int id)
        {
            #region Check Request
           
            #endregion Check Request

            var ret = await _testSubscriptionProcess.UpdateLicenseStatusOfUser(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Add/Update

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Delete(int id)
        {
            var ret = await _testSubscriptionProcess.DeleteUserSubscriptionById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Delete
    }
}
