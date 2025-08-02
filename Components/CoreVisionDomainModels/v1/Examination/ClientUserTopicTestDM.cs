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
        public double TotalMarks { get; set; }
    }
}
