using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class SubjectDM : CoreVisionDomainModelBase<int>
    {
        public string SubjectName { get; set; }

        public string SubjectDescription { get; set; }

        public ICollection<MCQDM> MCQs { get; set; }
        public ICollection<SubjectTopicDM> SubjectTopics { get; set; }
    }
}