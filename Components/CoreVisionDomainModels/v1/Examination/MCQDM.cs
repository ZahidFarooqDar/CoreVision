using CoreVisionDomainModels.Foundation.Base;

namespace CoreVisionDomainModels.v1.Examination
{
    public class MCQDM : CoreVisionDomainModelBase<int>
    {
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public string? Explanation { get; set; }

        public int? ExamId { get; set; }
        public ExamDM? Exam { get; set; }

        public int? SubjectId { get; set; }
        public SubjectDM? Subject { get; set; }

        public int? SubjectTopicId { get; set; }
        public SubjectTopicDM? SubjectTopic { get; set; }
    }
}
