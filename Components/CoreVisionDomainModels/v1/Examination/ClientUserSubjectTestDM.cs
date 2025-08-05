using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ClientUserSubjectTestDM : CoreVisionDomainModelBase<int>
    {
        [ForeignKey(nameof(Student))]
        public int UserId { get; set; }
        public virtual ClientUserDM Student { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }
        public virtual SubjectDM Subject { get; set; }
        public double? MarksObtained { get; set; }

        public int WrongAnswers { get; set; }
        public double? TotalMarks { get; set; }
        public bool? IsDataGet { get; set; }
        public bool? IsDataSubmitted { get; set; }
    }
}
