using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations;

namespace CoreVisionDomainModels.v1.General.ScanCodes
{
    public class ScanCodesFormatDM : CoreVisionDomainModelBase<int>
    {
        [Required]
        public string BarcodeFormat { get; set; }
        public string BarcodeFormatName { get; set; }
        public string Regex { get; set; }
        public string Description { get; set; }
        public string ErrorData { get; set; }
        public string ValidationMessage { get; set; }
    }
}
