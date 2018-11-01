using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Open.SPF.Utility;
using Open.SPF.Permissions;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for ConfigurationTests
    /// </summary>
    [TestClass]
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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

        [TestMethod]
        public void CoreConfigurationTest()
        {
            List<CorePermissionConfiguration> config = Open.SPF.Configuration.SpfConfiguration.CorePermissions;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Count > 0);

            foreach (CorePermissionConfiguration item in config)
            {
                Assert.IsNotNull(item);
            }
        }

        [TestMethod]
        public void CustomConfigurationTest()
        {
            List<CustomPermissionConfiguration> config = Open.SPF.Configuration.SpfConfiguration.CustomPermissions;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Count > 0);

            foreach (CustomPermissionConfiguration item in config)
            {
                Assert.IsNotNull(item);
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void RunBeforeAllTests(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void RunAfterAllTests() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void RunBeforeEachTest() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void RunAfterEachTest() { }
        //
        #endregion
    }
}
