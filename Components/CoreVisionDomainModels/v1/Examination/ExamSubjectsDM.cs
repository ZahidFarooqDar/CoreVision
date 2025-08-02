using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ExamSubjectsDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }
        public virtual ExamDM Exam { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }
        public virtual SubjectDM Subject { get; set; }
    }
}
