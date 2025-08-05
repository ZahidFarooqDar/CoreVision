using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ExamDM : CoreVisionDomainModelBase<int>
    {
        public string ExamName { get; set; }

        public string? ExamDescription { get; set; }
        public string ConductedBy { get; set; } = "JKSSB"; // Default to JKSSB
        public bool IsActive { get; set; } = true;
        public ICollection<MCQDM> MCQs { get; set; }

    }
}
