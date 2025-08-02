using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ExamDM : CoreVisionDomainModelBase<int>
    {
        public string ExamName { get; set; }

        public string ExamDescription { get; set; }
        public string ConductedBy { get; set; } = "JKSSB"; // Default to JKSSB
        public ICollection<MCQDM> MCQs { get; set; }
        public ICollection<SyllabusDataDM> SyllabusData { get; set; }
        public ICollection<ExamTopicDM> ExamSubjectTopics { get; set; }

    }
}
