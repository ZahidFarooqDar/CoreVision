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
    public class TestLicenseController : ApiControllerWithOdataRoot<TestLicenseTypeSM>
    {
        private readonly TestLicenseTypeProcess _testLicenseTypeProcess;
        public TestLicenseController(TestLicenseTypeProcess process)
            : base(process)
        {
            _testLicenseTypeProcess = process;
        }

        #region Odata EndPoints

        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TestLicenseTypeSM>>>> GetAsOdata(ODataQueryOptions<TestLicenseTypeSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata

        #region Get All/By Id and Count

        [HttpGet]
        [Route("GetAll")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TestLicenseTypeSM>>>> GetAll()
        {
            var retList = await _testLicenseTypeProcess.GetAllLicenses();
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<TestLicenseTypeSM>>> GetById(int id)
        {
            var ret = await _testLicenseTypeProcess.GetSingleLicenseDetailById(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpGet("Count")]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, ClientEmployee, CompanyAutomation")]
        public async Task<ActionResult<ApiResponse<IntResponseRoot>>> GetAllLicensesCount()
        {
            var ret = await _testLicenseTypeProcess.GetAllLicenseTypeCountResponse();
            return Ok(ModelConverter.FormNewSuccessResponse(new IntResponseRoot(ret, "Total License Types")));
        }

        #endregion Get

        #region Add/Update

        [HttpPost]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<TestLicenseTypeSM>>> Add([FromBody] ApiRequest<TestLicenseTypeSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _testLicenseTypeProcess.AddLicenseType(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<TestLicenseTypeSM>>> Update([FromBody] ApiRequest<TestLicenseTypeSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var ret = await _testLicenseTypeProcess.UpdateLicenseType(innerReq);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Add/Update

        #region Delete

        [HttpDelete]
        [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<BoolResponseRoot>>> Delete(int id)
        {
            var ret = await _testLicenseTypeProcess.DeleteLicenseType(id);
            return Ok(ModelConverter.FormNewSuccessResponse(ret));
        }

        #endregion Delete
    }
}
