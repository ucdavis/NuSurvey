using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuSurvey.Tests.InterfaceTests
{
    [TestClass]
    public class ScoreServiceTests
    {

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Do ScoreService Tests");
            /*
             * Answer tests:
             * Test when answer is a radio button:
             *  1) Response ID exists
             *  2) Score matches what the response id's score was.
             *  
             * When answer is open ended:
             *  when answer is an int
             *      1) score is calculated correctly exact match, + value, - value
             *      2) What about if value exists between + and -, but no exact match found?
             *  when answer is not an int
             *      What to do if date? what to do if some other answer type?
             */
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }

    }
}
