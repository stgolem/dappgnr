using AutoGen.GM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AGTest
{
    
    
    /// <summary>
    ///This is a test class for GMMathTest and is intended
    ///to contain all GMMathTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GMMathTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for VectDiff
        ///</summary>
        [TestMethod()]
        public void VectDiffTest()
        {
            List<double> v1 = null; // TODO: Initialize to an appropriate value
            List<double> v2 = null; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GMMath.VectDiff(v1, v2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RoundAcc
        ///</summary>
        [TestMethod()]
        public void RoundAccTest()
        {
            double value = 0F; // TODO: Initialize to an appropriate value
            double accuracy = 0F; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GMMath.RoundAcc(value, accuracy);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Norma
        ///</summary>
        [TestMethod()]
        public void NormaTest()
        {
            double[] vector = null; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GMMath.Norma(vector);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRealText
        ///</summary>
        [TestMethod()]
        public void GetRealTextTest()
        {
            double value = 0.000001; // TODO: Initialize to an appropriate value
            string expected = "0,0000010"; // TODO: Initialize to an appropriate value
            string actual;
            actual = GMMath.GetRealText(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRandom
        ///</summary>
        [TestMethod()]
        public void GetRandomTest1()
        {
            int valMin = 0; // TODO: Initialize to an appropriate value
            int valMax = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = GMMath.GetRandom(valMin, valMax);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetRandom
        ///</summary>
        [TestMethod()]
        public void GetRandomTest()
        {
            double valMin = 0F; // TODO: Initialize to an appropriate value
            double valMax = 0F; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GMMath.GetRandom(valMin, valMax);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DigitAfterComma
        ///</summary>
        [TestMethod()]
        public void DigitAfterCommaTest()
        {
            double number = 0F; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = GMMath.DigitAfterComma(number);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CalcToAccuracy
        ///</summary>
        [TestMethod()]
        public void CalcToAccuracyTest()
        {
            double value = 0F; // TODO: Initialize to an appropriate value
            double acc = 0F; // TODO: Initialize to an appropriate value
            double max = 0F; // TODO: Initialize to an appropriate value
            double min = 0F; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GMMath.CalcToAccuracy(value, acc, max, min);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
