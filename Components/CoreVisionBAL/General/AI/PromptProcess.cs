using AutoMapper;
using CoreVisionBAL.Foundation.Base;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionServiceModels.Foundation.Base.Interfaces;
using CoreVisionServiceModels.v1.Examination;

namespace CoreVisionBAL.General.AI
{
    public class PromptProcess : CoreVisionBalBase
    {
        
        public PromptProcess(IMapper mapper, ILoginUserDetail loginUserDetail, ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            
        }

        #region Prompt For MCQs

        public string GeneratePromptForMCQ(MCQSM objSM)
        {
            var prompt = $@"
                          You are an expert exam content analyst. Analyze the following multiple-choice question and provide a clear and concise explanation for the correct answer.
                          
                          Question:
                          {objSM.QuestionText}
                          
                          Options:
                          A. {objSM.OptionA}
                          B. {objSM.OptionB}
                          C. {objSM.OptionC}
                          D. {objSM.OptionD}
                          
                          Correct Answer: Option {objSM.CorrectOption}

                          Instructions:
                          1. Explain the correct answer.
                          2. Explain the base of question with some description.
                          3. Keep the explanation factual and educational.
                          4. Response should be strictly a text response and nothing else
                          ";

            return prompt.Trim();
        }


        #endregion Prompt For MCQs
    }
}
