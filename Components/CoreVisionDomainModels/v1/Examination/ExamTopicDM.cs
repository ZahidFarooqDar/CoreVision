using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreVisionDomainModels.v1.Examination
{
    public class ExamTopicDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }
        public virtual ExamDM Exam { get; set; }       

        [ForeignKey(nameof(SubjectTopic))]
        public int SubjectTopicId { get; set; }
        public virtual SubjectTopicDM SubjectTopic { get; set; }
    }
}
