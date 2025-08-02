using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ClientUserExamTestDM : CoreVisionDomainModelBase<int>
    {
        [ForeignKey(nameof(Student))]
        public int UserId { get; set; }
        public virtual ClientUserDM Student { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }
        public virtual ExamDM Exam { get; set; }
        public double MarksObtained { get; set; }

        public double TotalMarks { get; set; }
    }
}
