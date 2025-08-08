using AutoMapper;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.Foundation.Base;
using CoreVisionBAL.License;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionDomainModels.v1.Examination;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.Examination;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionBAL.General.Examination
{
    public class UserExamProcess : CoreVisionBalBase
    {
        #region Properties

        private readonly ILoginUserDetail _loginUserDetail;
        private readonly MCQProcess _mcqProcess;
        private readonly APIConfiguration _apiConfiguration;
        private readonly UserTestLicenseDetailsProcess _userTestLicenseDetailsProcess;

        #endregion Properties

        #region Constructor
        public UserExamProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext, UserTestLicenseDetailsProcess userTestLicenseDetailsProcess,
            APIConfiguration configuration, MCQProcess mcqProcess)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
            _mcqProcess = mcqProcess;
            _apiConfiguration = configuration;
            _userTestLicenseDetailsProcess = userTestLicenseDetailsProcess;
        }

        #endregion Constructor

        #region Eligible For Test
        public async Task<BoolResponseRoot> IsUserEligibleForSubjectTest(int subjectId, int userId)
        {
            var totalEligibleTests = await _userTestLicenseDetailsProcess.GetActiveUserLicenseTestCountsByUserId(userId);
            var testCount = await _apiDbContext.ClientSubjectTests.Where(x => x.SubjectId == subjectId && x.UserId == userId).CountAsync();
            if (testCount <= totalEligibleTests)
            {
                return new BoolResponseRoot(true, "User is eligible for test.");                
            }
            else
            {
                return new BoolResponseRoot(false, "User is not eligible for test.");
            }
        }

        public async Task<BoolResponseRoot> IsUserEligibleForExamTest(int examId, int userId)
        {
            var totalEligibleTests = await _userTestLicenseDetailsProcess.GetActiveUserLicenseTestCountsByUserId(userId);
            var testCount = await _apiDbContext.ClientExamTests.Where(x => x.ExamId == examId && x.UserId == userId).CountAsync();
            if (testCount <= totalEligibleTests)
            {
                return new BoolResponseRoot(true, "User is eligible for test.");
            }
            else
            {
                return new BoolResponseRoot(false, "User is not eligible for test.");
            }
        }

        public async Task<BoolResponseRoot> IsUserEligibleForSubjectTopicTest(int topicId, int userId)
        {
            var totalEligibleTests = await _userTestLicenseDetailsProcess.GetActiveUserLicenseTestCountsByUserId(userId);
            var testCount = await _apiDbContext.ClientTopicTests.Where(x => x.SubjectTopicId == topicId && x.UserId == userId).CountAsync();
            if (testCount <= totalEligibleTests)
            {
                return new BoolResponseRoot(true, "User is eligible for test.");
            }
            else
            {
                return new BoolResponseRoot(false, "User is not eligible for test.");
            }
        }

        #endregion Eligible For Test

        #region User Test Marks For Subject
        public async Task<IntResponseRoot> UserSubjectTestMarksGetRequest(int subjectId, int userId)
        {
            if (subjectId < 1 || userId < 1)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "SubjectId or UserId is invalid", "Something went wrong, Please try again later");
            }
            var existingSubject = await _apiDbContext.Subjects.FindAsync(subjectId);
            if (existingSubject != null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"SubjectId: {subjectId} is invalid", "Something went wrong, Please try again later");
            }

            var dm = new ClientUserSubjectTestDM
            {
                UserId = userId,
                SubjectId = subjectId,
                IsDataGet = true,
                IsDataSubmitted = false,
                MarksObtained = 0,
                TotalMarks = 0,
                WrongAnswers = 0,
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = DateTime.UtcNow
            };
            await _apiDbContext.ClientSubjectTests.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new IntResponseRoot(dm.Id, "User Subject Test Marks get successfully");
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add User Subject Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        public async Task<TestResponseSM> UserSubjectTestMarks(int subjectTestId, List<MCQSM> mcqs, int userId)
        {
            if (mcqs == null || mcqs.Count < 1)
            {
                return null;
            }
            var existingTest = await _apiDbContext.ClientSubjectTests.FindAsync(subjectTestId);
            if (existingTest == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject Test Id is invalid: {subjectTestId}, possibility of data tampering", "Something went wrong, Please try again later");
            }
            if(existingTest.IsDataGet == false)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test without getting test", "Something went wrong, Please try again later");
            }
            if(existingTest.IsDataSubmitted == true)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test again", "Subject Test is already submitted");
            }
            int marksCount = 0;
            int wrongAnswersCount = 0;
            foreach (var mcq in mcqs)
            {
                var isCorrectOption = await _mcqProcess.IsAnswerCorrect(mcq.Id, mcq.CorrectOptionByUser);
                if (isCorrectOption)
                {
                    marksCount++;
                }
                else
                {
                    wrongAnswersCount++;
                }
            }            
            existingTest.MarksObtained = marksCount;
            existingTest.TotalMarks = mcqs.Count;
            existingTest.WrongAnswers = wrongAnswersCount;
            existingTest.IsDataSubmitted = true;
            existingTest.LastModifiedOnUTC = DateTime.UtcNow;
            existingTest.LastModifiedBy = _loginUserDetail.LoginId;

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new TestResponseSM
                {
                    TotalMarksObtained = marksCount,
                    TotalMarks = mcqs.Count,
                    WrongAnsersCount = wrongAnswersCount
                };
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add User Subject Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        #endregion User Test Marks For Subject

        #region User Test Marks For Exam

        public async Task<IntResponseRoot> UserExamTestMarksGetRequest(int examId, int userId)
        {
            if (examId < 1 || userId < 1)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "ExamId or UserId is invalid", "Something went wrong, Please try again later");
            }
            var existingExam = await _apiDbContext.Exams.FindAsync(examId);
            if (existingExam == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exam Id is invalid: {examId}, possibility of data tampering", "Something went wrong, Please try again later");
            }
            var dm = new ClientUserExamTestDM
            {
                UserId = userId,
                ExamId = examId,
                IsDataGet = true,
                IsDataSubmitted = false,
                MarksObtained = 0,
                TotalMarks = 0,
                WrongAnswers = 0,
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = DateTime.UtcNow
            };
            await _apiDbContext.ClientExamTests.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new IntResponseRoot(dm.Id, "User Exam Test Marks get successfully");
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add User Subject Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        public async Task<TestResponseSM> UserExamTestMarks(int examTestId, List<MCQSM> mcqs, int userId)
        {
            if (mcqs == null || mcqs.Count < 1)
            {
                return null;
            }
            var existingTest = await _apiDbContext.ClientExamTests.FindAsync(examTestId);
            if (existingTest == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exam Test Id is invalid: {examTestId}, possibility of data tampering", "Something went wrong, Please try again later");
            }
            if (existingTest.IsDataGet == false)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test without getting test", "Something went wrong, Please try again later");
            }
            if (existingTest.IsDataSubmitted == true)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test again", "Subject Test is already submitted");
            }

            int marksCount = 0;
            int wrongAnswersCount = 0;
            foreach (var mcq in mcqs)
            {
                var isCorrectOption = await _mcqProcess.IsAnswerCorrect(mcq.Id, mcq.CorrectOptionByUser);
                if (isCorrectOption)
                {
                    marksCount++;
                }
                else
                {
                    wrongAnswersCount++;
                }
            }
            existingTest.MarksObtained = marksCount;
            existingTest.TotalMarks = mcqs.Count;
            existingTest.WrongAnswers = wrongAnswersCount;
            existingTest.IsDataSubmitted = true;
            existingTest.LastModifiedOnUTC = DateTime.UtcNow;
            existingTest.LastModifiedBy = _loginUserDetail.LoginId;

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new TestResponseSM
                {
                    TotalMarksObtained = marksCount,
                    TotalMarks = mcqs.Count,
                    WrongAnsersCount = wrongAnswersCount
                };
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add User Subject Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        #endregion User Test Marks For Exam

        #region User Test Marks For Subject Topic

        public async Task<IntResponseRoot> UserTopicTestMarksGetRequest(int topicId, int userId)
        {
            if (topicId < 1 || userId < 1)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "ExamId or UserId is invalid", "Something went wrong, Please try again later");
            }
            var existingTopic = await _apiDbContext.SubjectTopics.FindAsync(topicId);
            if (existingTopic == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Topic Id is invalid: {topicId}, possibility of data tampering", "Something went wrong, Please try again later");
            }
            var dm = new ClientUserTopicTestDM
            {
                UserId = userId,
                SubjectTopicId = topicId,
                IsDataGet = true,
                IsDataSubmitted = false,
                MarksObtained = 0,
                TotalMarks = 0,
                WrongAnswers = 0,
                CreatedBy = _loginUserDetail.LoginId,
                CreatedOnUTC = DateTime.UtcNow
            };
            await _apiDbContext.ClientTopicTests.AddAsync(dm);

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new IntResponseRoot(dm.Id, "User Topic Test Marks get successfully");
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add Subject Topic Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        public async Task<TestResponseSM> UserTopicTestMarks(int topicTestId, List<MCQSM> mcqs, int topicId, int userId)
        {
            if (mcqs == null || mcqs.Count < 1)
            {
                return null;
            }
            var existingTest = await _apiDbContext.ClientTopicTests.FindAsync(topicTestId);
            if (existingTest == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Topic Test Id is invalid: {topicTestId}, possibility of data tampering", "Something went wrong, Please try again later");
            }
            if (existingTest.IsDataGet == false)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test without getting test", "Something went wrong, Please try again later");
            }
            if (existingTest.IsDataSubmitted == true)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"User with Id:{userId}, tries to submit test again", "Subject Test is already submitted");
            }
            int marksCount = 0;
            int wrongAnswersCount = 0;
            foreach (var mcq in mcqs)
            {
                var isCorrectOption = await _mcqProcess.IsAnswerCorrect(mcq.Id, mcq.CorrectOptionByUser);
                if (isCorrectOption)
                {
                    marksCount++;
                }
                else
                {
                    wrongAnswersCount++;
                }
            }
            existingTest.MarksObtained = marksCount;
            existingTest.TotalMarks = mcqs.Count;
            existingTest.WrongAnswers = wrongAnswersCount;
            existingTest.IsDataSubmitted = true;
            existingTest.LastModifiedOnUTC = DateTime.UtcNow;
            existingTest.LastModifiedBy = _loginUserDetail.LoginId;

            if (await _apiDbContext.SaveChangesAsync() > 0)
            {
                return new TestResponseSM
                {
                    TotalMarksObtained = marksCount,
                    TotalMarks = mcqs.Count,
                    WrongAnsersCount = wrongAnswersCount
                };
            }
            throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                "Failed to add User Topic Test Marks. See inner exception.",
                "Something went wrong, Please try again");
        }

        #endregion User Test Marks For Subject Topic

        #region User Test Results

        public async Task<UserTestReportSM> UserExamTestReport(int userId, int examId)
        {
            if(examId < 1)
            {
                return null;
            }
            var examDm = await _apiDbContext.Exams.FindAsync(examId);
            if(examDm == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Exams Details not found for id: {examId}", "Something went wrong, Please try again later");
            }
            var dm = await _apiDbContext.ClientExamTests.Where(x => x.ExamId == examId && x.UserId == userId).ToListAsync();
            if(dm.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "User Exam Test Report not found", "Test Report not found, Please solve test first");
            }
            var testReport = new List<UserTestReportDetailsSM>();
            foreach(var item in dm)
            {
                var testDetails = new UserTestReportDetailsSM();
                testDetails.ObtainedMarks = item.TotalMarks;
                testDetails.TotalMarks = item.MarksObtained;   
                testReport.Add(testDetails);
            }
            return new UserTestReportSM
            {
                TestName = examDm.ExamName,
                UserTestReportDetails = testReport,
                Percentage = Math.Round(((double)dm.Sum(x => x.MarksObtained) / (double)dm.Sum(x => x.TotalMarks)) * 100, 2)
            };

        }

        public async Task<UserTestReportSM> UserSubjectTestReport(int userId, int subjectId)
        {
            if (subjectId < 1)
            {
                return null;
            }
            var subDM = await _apiDbContext.Subjects.FindAsync(subjectId);
            if (subDM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Subject Details not found for id: {subjectId}", "Something went wrong, Please try again later");
            }
            var dm = await _apiDbContext.ClientSubjectTests.Where(x => x.SubjectId == subjectId && x.UserId == userId ).ToListAsync();
            if (dm.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "User Subject Test Report not found", "Test Report not found, Please solve test first");
            }
            var testReport = new List<UserTestReportDetailsSM>();
            foreach (var item in dm)
            {
                var testDetails = new UserTestReportDetailsSM();
                testDetails.ObtainedMarks = (double)item.MarksObtained;
                testDetails.TotalMarks = (double)item.MarksObtained;
                testReport.Add(testDetails);
            }
            return new UserTestReportSM
            {
                UserTestReportDetails = testReport,
                Percentage = Math.Round(((double)dm.Sum(x => x.MarksObtained) / (double)dm.Sum(x => x.TotalMarks)) * 100, 2)
            };

        }

        public async Task<UserTestReportSM> UserTopicTestReport(int userId, int topicId)
        {
            if (topicId < 1)
            {
                return null;
            }
            var topicDM = await _apiDbContext.SubjectTopics.FindAsync(topicId);
            if (topicDM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Topic Details not found for id: {topicId}", "Something went wrong, Please try again later");
            }
            var dm = await _apiDbContext.ClientTopicTests.Where(x => x.SubjectTopicId == topicId && x.UserId == userId).ToListAsync();
            if (dm.Count == 0)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "User Topic Test Report not found", "Test Report not found, Please solve test first");
            }
            var testReport = new List<UserTestReportDetailsSM>();
            foreach (var item in dm)
            {
                var testDetails = new UserTestReportDetailsSM();
                testDetails.ObtainedMarks = item.TotalMarks;
                testDetails.TotalMarks = item.MarksObtained;
                testReport.Add(testDetails);
            }
            return new UserTestReportSM
            {
                UserTestReportDetails = testReport,
                Percentage = Math.Round(((double)dm.Sum(x => x.MarksObtained) / (double)dm.Sum(x => x.TotalMarks)) * 100, 2)
            };

        }

        #endregion User Test Results

        #region User Top Performances

        public async Task<(string TestName, double Percentage)> GetUserTopExamPerformance(int userId)
        {
            // Fetch all exam tests for user
            var examTests = await _apiDbContext.ClientExamTests
                .Where(x => x.UserId == userId && x.TotalMarks > 0)
                .ToListAsync();

            if (!examTests.Any())
                return ( "", 0);

            // Calculate percentages for each exam test grouped by ExamId
            var grouped = examTests
                .GroupBy(x => x.ExamId)
                .Select(g => new
                {
                    ExamId = g.Key,
                    MarksObtained = g.Sum(x => x.MarksObtained),
                    TotalMarks = g.Sum(x => x.TotalMarks),
                    Percentage = (double)g.Sum(x => x.MarksObtained) / g.Sum(x => x.TotalMarks) * 100
                })
                .OrderByDescending(x => x.Percentage)
                .FirstOrDefault();

            if (grouped == null)
                return ("", 0);

            var exam = await _apiDbContext.Exams.FindAsync(grouped.ExamId);

            return (exam?.ExamName, Math.Round(grouped.Percentage, 2));
        }

        public async Task<(string SubjectName, double Percentage)> GetUserTopSubjectPerformance(int userId)
        {
            var subjectTests = await _apiDbContext.ClientSubjectTests
                .Where(x => x.UserId == userId && x.TotalMarks.GetValueOrDefault() > 0)
                .ToListAsync();

            if (!subjectTests.Any())
                return ("", 0);

            var grouped = subjectTests
                .GroupBy(x => x.SubjectId)
                .Select(g => new
                {
                    SubjectId = g.Key,
                    MarksObtained = g.Sum(x => x.MarksObtained).GetValueOrDefault(),
                    TotalMarks = g.Sum(x => x.TotalMarks).GetValueOrDefault(),
                    Percentage = (double)(g.Sum(x => x.MarksObtained).GetValueOrDefault()) / g.Sum(x => x.TotalMarks).GetValueOrDefault() * 100
                })
                .OrderByDescending(x => x.Percentage)
                .FirstOrDefault();

            if (grouped == null)
                return ("", 0);

            var subject = await _apiDbContext.Subjects.FindAsync(grouped.SubjectId);

            // Math.Round for double (non-nullable) property
            return (subject?.SubjectName, Math.Round(grouped.Percentage, 2));
        }

        public async Task<(string TopicName, double Percentage)> GetUserTopTopicPerformance(int userId)
        {
            var topicTests = await _apiDbContext.ClientTopicTests
                .Where(x => x.UserId == userId && x.TotalMarks > 0)
                .ToListAsync();

            if (!topicTests.Any())
                return ("", 0);

            var grouped = topicTests
                .GroupBy(x => x.SubjectTopicId)
                .Select(g => new
                {
                    TopicId = g.Key,
                    MarksObtained = g.Sum(x => x.MarksObtained),
                    TotalMarks = g.Sum(x => x.TotalMarks),
                    Percentage = (double)g.Sum(x => x.MarksObtained) / g.Sum(x => x.TotalMarks) * 100
                })
                .OrderByDescending(x => x.Percentage)
                .FirstOrDefault();

            if (grouped == null)
                return ("", 0);

            var topic = await _apiDbContext.SubjectTopics.FindAsync(grouped.TopicId);

            return (topic?.TopicName, Math.Round(grouped.Percentage, 2));
        }

        #endregion

    }
}
