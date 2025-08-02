using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class SubjectTopicDM : CoreVisionDomainModelBase<int>
    {
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }
        public int SubjectId { get; set; }
        public virtual SubjectDM Subject { get; set; }
        public ICollection<MCQDM> MCQs { get; set; }
        public ICollection<SyllabusDataDM> SyllabusData { get; set; }
        public ICollection<ExamTopicDM> ExamSubjectTopics { get; set; }
    }
}
