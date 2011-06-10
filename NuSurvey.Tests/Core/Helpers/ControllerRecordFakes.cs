using System.Collections.Generic;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.Core.Helpers
{
    public class FakeAnswers : ControllerRecordFakes<Answer>
    {
        protected override Answer CreateValid(int i)
        {
            return CreateValidEntities.Answer(i);
        }
    }

    public class FakeSurveys : ControllerRecordFakes<Survey>
    {
        protected override Survey CreateValid(int i)
        {
            return CreateValidEntities.Survey(i);
        }
        public FakeSurveys(int count, IRepository<Survey> survey, List<Survey> specificRecords)
        {
            Records(count, survey, specificRecords);
        }

        public FakeSurveys(int count, IRepository<Survey> survey)
        {
            Records(count, survey);
        }
        public FakeSurveys()
        {
            
        }
    }

    public class FakeSurveyResponses : ControllerRecordFakes<SurveyResponse>
    {
        protected override SurveyResponse CreateValid(int i)
        {
            return CreateValidEntities.SurveyResponse(i);
        }

        public FakeSurveyResponses(int count, IRepository<SurveyResponse> surveyResponse, List<SurveyResponse> specificRecords)
        {
            Records(count, surveyResponse, specificRecords);
        }

        public FakeSurveyResponses(int count, IRepository<SurveyResponse> surveyResponse)
        {
            Records(count, surveyResponse);
        }
    }

    public class FakeCategories : ControllerRecordFakes<Category>
    {
        protected override Category CreateValid(int i)
        {
            return CreateValidEntities.Category(i);
        }
    }

    public class FakeCategoryGoals : ControllerRecordFakes<CategoryGoal>
    {
        protected override CategoryGoal CreateValid(int i)
        {
            return CreateValidEntities.CategoryGoal(i);
        }
    }
}
