using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ClientUserTopicTestDM : CoreVisionDomainModelBase<int>
    {
        [ForeignKey(nameof(Student))]
        public int UserId { get; set; }
        public virtual ClientUserDM Student { get; set; }

        [ForeignKey(nameof(SubjectTopic))]
        public int SubjectTopicId { get; set; }
        public virtual SubjectTopicDM SubjectTopic { get; set; }
        public double MarksObtained { get; set; }

        public double WrongAnswers { get; set; }
        public double TotalMarks { get; set; }

        public bool IsDataGet { get; set; }
        public DateTime? FetchedOnUtc { get; set; }
        public DateTime? SubmittedOnUtc { get; set; }
        public bool IsDataSubmitted { get; set; }
    }
}
