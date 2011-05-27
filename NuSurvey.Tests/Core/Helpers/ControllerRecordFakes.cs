using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuSurvey.Core.Domain;
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
    }
}
