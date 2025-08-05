namespace CoreVisionDomainModels.v1.Examination
{
    public class UserTestReportSM
    {
        public string TestName { get; set; }
        public double Percentage { get; set; }

        public List<UserTestReportDetailsSM> UserTestReportDetails { get; set; } 
    }
}
