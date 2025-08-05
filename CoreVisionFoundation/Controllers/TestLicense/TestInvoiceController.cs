using CoreVisionBAL.Foundation.Web;
using CoreVisionBAL.License;
using CoreVisionBAL.Projects.ScanCode;
using CoreVisionFoundation.Controllers.Base;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.License;
using CoreVisionServiceModels.v1.General.ScanCodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CoreVisionFoundation.Controllers.TestLicense
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestInvoiceController : ApiControllerWithOdataRoot<UserTestInvoiceSM>
    {
        private readonly UserTestInvoiceProcess _invoiceProcess;
        public TestInvoiceController(UserTestInvoiceProcess process)
            : base(process)
        {
            _invoiceProcess = process;
        }

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestInvoiceSM>>>> GetAsOdata(ODataQueryOptions<UserTestInvoiceSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata

        #region Get All/By Id and Count

        [HttpGet]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestInvoiceSM>>>> GetAll()
        {
            var retList = await _invoiceProcess.GetAllUserInvoices();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("mine")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "ClientEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTestInvoiceSM>>>> GetAllMineInvoices()
        {
            #region Check Request

            var userId = User.GetUserRecordIdFromCurrentUserClaims();

            if (userId <= 0)
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_IdNotFound));
            }

            #endregion Check Request
            var retList = await _invoiceProcess.GetMineInvoices(userId);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestInvoiceSM>>> GetById(int id)
        {
            var ret = await _invoiceProcess.GetUserInvoiceById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("Count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllLicensesCount()
        {
            var ret = await _invoiceProcess.GetAllUserInvoicesCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total License Types")));
        }

        #endregion Get

        #region Add/Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestInvoiceSM>>> Add([FromBody] ApiRequest<UserTestInvoiceSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _invoiceProcess.AddUserInvoice(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<UserTestInvoiceSM>>> Update(int id, [FromBody] ApiRequest<UserTestInvoiceSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _invoiceProcess.UpdateUserInvoice(id,innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Add/Update

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Delete(int id)
        {
            var ret = await _invoiceProcess.DeleteUserInvoiceById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Delete
    }
}
