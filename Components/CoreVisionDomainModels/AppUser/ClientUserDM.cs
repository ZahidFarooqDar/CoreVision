using CoreVisionDomainModels.AppUser.Login;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoreVisionDomainModels.Client;
using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.v1.Examination;

namespace CoreVisionDomainModels.AppUser
{
    public class ClientUserDM : LoginUserDM
    {
        public ClientUserDM()
        {
        }
        public GenderDM? Gender { get; set; }

        [ForeignKey(nameof(ClientCompanyDetail))]
        public int? ClientCompanyDetailId { get; set; }
        public virtual ClientCompanyDetailDM? ClientCompanyDetail { get; set; }
        public ICollection<ClientUserExamTestDM> ExamTestDeatils { get; set; }
        public ICollection<ClientUserSubjectTestDM> SubjectTestDeatils { get; set; }
        public ICollection<ClientUserTopicTestDM> TopicTestDeatils { get; set; }

    }
}
