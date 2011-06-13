using System;
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
        public FakeAnswers(int count, IRepository<Answer> repository, List<Answer> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeAnswers(int count, IRepository<Answer> repository)
        {
            Records(count, repository);
        }
        public FakeAnswers()
        {
            
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
        public FakeCategories(int count, IRepository<Category> repository, List<Category> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeCategories(int count, IRepository<Category> repository)
        {
            Records(count, repository);
        }
        public FakeCategories()
        {
            
        }
    }

    public class FakeCategoryGoals : ControllerRecordFakes<CategoryGoal>
    {
        protected override CategoryGoal CreateValid(int i)
        {
            return CreateValidEntities.CategoryGoal(i);
        }
    }

    public class FakeQuestions : ControllerRecordFakes<Question>
    {
        protected override Question CreateValid(int i)
        {
            return CreateValidEntities.Question(i);
        }

        public FakeQuestions(int count, IRepository<Question> repository, List<Question> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeQuestions(int count, IRepository<Question> repository)
        {
            Records(count, repository);
        }
    }
}
