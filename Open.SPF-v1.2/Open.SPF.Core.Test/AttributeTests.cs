using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for AttributeTests
    /// </summary>
    [TestClass]
    public class AttributeTests
    {
        public AttributeTests()
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
        public void MethodAttributeTest()
        {
            TypeInfo ruleClass = typeof(TestCustomRules).GetTypeInfo();
            MethodInfo[] ruleMethods = ruleClass.GetMethods();

            int ruleCount = 0;
            foreach (MethodInfo ruleMethod in ruleMethods)
            {
                if (ruleMethod.Name == "CustomRuleWrongSignatureRule")
                    continue; // ignore this rule - it's known to be invalid

                PermissionRuleMethodAttribute permissionAttribute = ruleMethod.GetCustomAttribute<PermissionRuleMethodAttribute>();
                if (permissionAttribute != null)
                {
                    ruleCount++;
                    ParameterInfo[] parmsInfo = ruleMethod.GetParameters();
                    Assert.AreEqual(4, parmsInfo.Length);
                    Assert.AreEqual(typeof(System.Security.Principal.IIdentity), parmsInfo[0].ParameterType);
                    Assert.AreEqual(typeof(IEnumerable<string>), parmsInfo[1].ParameterType);
                    Assert.AreEqual(typeof(object), parmsInfo[2].ParameterType);
                    Assert.AreEqual(typeof(Dictionary<string, object>), parmsInfo[3].ParameterType);
                }
            }
            Assert.AreEqual(9, ruleCount);
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
