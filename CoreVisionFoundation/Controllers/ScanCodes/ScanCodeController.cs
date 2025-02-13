using CoreVisionBAL.Projects.ScanCode;
using CoreVisionFoundation.Controllers.Base;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.AppUser;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.v1.General.ScanCodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ZXing;

namespace CoreVisionFoundation.Controllers.ScanCodes
{
    [ApiController]
    [Route("api/v1/[controller]")]

    [Authorize(AuthenticationSchemes = CoreVisionBearerTokenAuthHandlerRoot.DefaultSchema, Roles = "SuperAdmin, SystemAdmin, ClientAdmin")]
    public class ScanCodeController : ApiControllerWithOdataRoot<ScanCodesFormatSM>
    {
        private readonly ScanCodesProcess _scanCodesProcess;
        public ScanCodeController(ScanCodesProcess process)
            : base(process)
        {
            _scanCodesProcess = process;
        }

        #region Odata EndPoints
        [HttpGet]
        [Route("odata")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ScanCodesFormatSM>>>> GetAsOdata(ODataQueryOptions<ScanCodesFormatSM> oDataOptions)
        {
            //TODO: validate inputs here probably 
            var retList = await GetAsEntitiesOdata(oDataOptions);
            return Ok(ModelConverter.FormNewSuccessResponse(retList));
        }

        #endregion Odata EndPoints

        #region Get All

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ScanCodesFormatSM>>>> GetAll()
        {
            var list = await _scanCodesProcess.GetAllScanCodes();
            return Ok(ModelConverter.FormNewSuccessResponse(list));
        }

        #endregion Get All

        #region Get By Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ScanCodesFormatSM>>> GetById(int id)
        {
            var objSM = await _scanCodesProcess.GetScanCodeById(id);
            if (objSM != null)
            {
                return ModelConverter.FormNewSuccessResponse(objSM);
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_IdNotFound, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        #endregion Get By Id

        #region Add

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ScanCodesFormatSM>>> Post([FromBody] ApiRequest<ScanCodesFormatSM> apiRequest)
        {
            #region Check Request

            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }
            #endregion Check Request

            var addedSM = await _scanCodesProcess.AddBarcodeFormat(innerReq);
            if (addedSM != null)
            {
                return CreatedAtAction(nameof(GetById), new
                {
                    id = addedSM.Id
                }, ModelConverter.FormNewSuccessResponse(addedSM));
            }
            else
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }
        #endregion Add

        #region Put
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ScanCodesFormatSM>>> Put(int id, [FromBody] ApiRequest<ScanCodesFormatSM> apiRequest)
        {
            #region Check Request
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            if (id <= 0)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_IdInvalid, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            #endregion Check Request

            var resp = await _scanCodesProcess.UpdateBarcodeDetails(id, innerReq);
            if (resp != null)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(DomainConstantsRoot.DisplayMessagesRoot.Display_PassedDataNotSaved, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }
        #endregion Put

        #region Delete

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<DeleteResponseRoot>>> Delete(int id)
        {
            var resp = await _scanCodesProcess.DeleteBarcodeFormatById(id);
            if (resp != null && resp.DeleteResult)
            {
                return Ok(ModelConverter.FormNewSuccessResponse(resp));
            }
            else
            {
                return NotFound(ModelConverter.FormNewErrorResponse(resp?.DeleteMessage, ApiErrorTypeSM.NoRecord_NoLog));
            }
        }

        #endregion Delete

        #region Generate QrCode

        [HttpPost("qrcode")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CodeResponseSM>>> GenerateQRCode([FromBody] ApiRequest<GenerateQRCodeSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            // Generate the barcode image (assuming it returns a byte array).
            var barcodeImageData = await _scanCodesProcess.GenerateQRcode(innerReq);

            return ModelConverter.FormNewSuccessResponse(barcodeImageData);

        }

        #endregion Generate QrCode

        #region Zxing  Codes

        [HttpPost("generate")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CodeResponseSM>>> GenerateBarcodes([FromBody] ApiRequest<GenerateBarcodeSM> apiRequest)
        {
            var innerReq = apiRequest?.ReqData;
            if (innerReq == null)
            {
                return BadRequest(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessagesRoot.Display_ReqDataNotFormed, ApiErrorTypeSM.InvalidInputData_NoLog));
            }

            // Generate the barcode image (assuming it returns a byte array).
            var barcodeImageData = await _scanCodesProcess.GenerateCode(innerReq);

            return ModelConverter.FormNewSuccessResponse(barcodeImageData);

        }

        #endregion Zxing
    }
}
