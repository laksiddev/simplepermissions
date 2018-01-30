using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.SPF.Configuration;

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
            object uut = System.Configuration.ConfigurationManager.GetSection("appPermissionConfiguration");

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(ApplicationPermissionConfigurationSettings));

            ApplicationPermissionConfigurationSettings config = (ApplicationPermissionConfigurationSettings)uut;
            foreach (object item in config.CorePermissionConfigurationItems)
            {
                Assert.IsNotNull(item);
                Assert.IsInstanceOfType(item, typeof(CorePermissionConfigurationElement));
            }
        }

        [TestMethod]
        public void CustomConfigurationTest()
        {
            object uut = System.Configuration.ConfigurationManager.GetSection("appPermissionConfiguration");

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(ApplicationPermissionConfigurationSettings));

            ApplicationPermissionConfigurationSettings config = (ApplicationPermissionConfigurationSettings)uut;
            foreach (object item in config.CustomPermissionConfigurationItems)
            {
                Assert.IsNotNull(item);
                Assert.IsInstanceOfType(item, typeof(CustomPermissionConfigurationElement));
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
