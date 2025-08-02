using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class SyllabusDataDM : CoreVisionDomainModelBase<int>
    {
        public string Title { get; set; }
        public string FileUrl { get; set; }        

        public int? ExamId { get; set; }
        public ExamDM? Exam { get; set; }

        public int? SubjectId { get; set; }
        public SubjectDM? Subject { get; set; }

        public int? SubjectTopicId { get; set; }
        public SubjectTopicDM? SubjectTopic { get; set; }
    }
}
