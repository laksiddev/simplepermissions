using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using Open.SPF.Permissions;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for RuleInvocationStateTests
    /// </summary>
    [TestClass]
    public class PermissionRuleContextTests
    {
        public PermissionRuleContextTests()
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
        public void ParallelRulesTest()
        {
            List<Task<bool?>> ruleTasks = new List<Task<bool?>>();
            TestCustomRules customRules = new TestCustomRules();
            PermissionRules uut = new PermissionRules();
            Open.Common.Configuration.CommonConfiguration test = new Common.Configuration.CommonConfiguration();

            PermissionRules.IsAuthorizedMethod del1 = new PermissionRules.IsAuthorizedMethod(customRules.CustomRuleIsAlwaysAuthorized);
            Dictionary<string, object> contextPropertyBag1 = customRules.PreparePropertiesForRule("CustomRuleIsAlwaysAuthorized", null);
            PermissionRuleContext ruleState1 = new PermissionRuleContext("CustomRuleIsAlwaysAuthorized", "CanDoStuff", contextPropertyBag1, del1);
            Task<bool?> task1 = uut.TaskFromPermissionRuleContext(ruleState1, PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles);

            PermissionRules.IsAuthorizedMethod del2 = new PermissionRules.IsAuthorizedMethod(customRules.CustomRuleIsNeverAuthorized);
            Dictionary<string, object> contextPropertyBag2 = customRules.PreparePropertiesForRule("CustomRuleIsNeverAuthorized", null);
            PermissionRuleContext ruleState2 = new PermissionRuleContext("CustomRuleIsNeverAuthorized", "MayNotDoThisStuff", contextPropertyBag2, del2);
            Task<bool?> task2 = uut.TaskFromPermissionRuleContext(ruleState2, PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles);

            task1.Start();
            ruleTasks.Add(task1);

            task2.Start();
            ruleTasks.Add(task2);

            // It's almost pointless because both tasks have likely finished by this point, but it's worth excercising the proper code
            Task.WaitAll(ruleTasks.ToArray());

            bool wasAlwaysRuleFound = false;
            bool wasNeverRuleFound = false;
            foreach (Task<bool?> ruleTask in ruleTasks)
            {
                if ((Guid)ruleTask.AsyncState == ruleState1.ContextId)
                {
                    Assert.IsTrue(ruleTask.Result.HasValue);
                    Assert.IsTrue(ruleTask.Result.Value);
                    wasAlwaysRuleFound = true;
                }
                else if ((Guid)ruleTask.AsyncState == ruleState2.ContextId)
                {
                    Assert.IsTrue(ruleTask.Result.HasValue);
                    Assert.IsFalse(ruleTask.Result.Value);
                    wasNeverRuleFound = true;
                }
            }
            Assert.IsTrue(wasAlwaysRuleFound && wasNeverRuleFound);
        }

        [TestMethod]
        public void RuleFromTypeAndMethodNameTest()
        {
            string typeName = "Open.SPF.Core.Test.TestCustomRules, Open.SPF.Core.Test, Version=2.0, Culture=neutral, PublicKeyToken=b4313207536550be";
            string methodName = "CustomRuleIsAlwaysAuthorized";

            PermissionRules uut = new PermissionRules();
            ICustomRuleContainer customContainer = uut.CustomRuleContainerFromTypeName(typeName);
            Assert.IsNotNull(customContainer);
            PermissionRules.IsAuthorizedMethod del = uut.RuleDelegateFromMethodName(customContainer, methodName);
            Assert.IsNotNull(del);

            string ruleName = uut.RuleNameFromMethodName(customContainer, methodName);
            Assert.IsNotNull(ruleName);
            Assert.AreEqual(methodName, ruleName);
            Dictionary<string, object> contextPropertyBag = customContainer.PreparePropertiesForRule(ruleName, null);

            bool? isAuthorized = del.Invoke(PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);
        }

        [TestMethod]
        public void ContextObjectTest()
        {
            string methodName = "CustomRuleAreEqualRule";

            PermissionRules uut = new PermissionRules();
            ICustomRuleContainer customContainer = new TestCustomRules();
            PermissionRules.IsAuthorizedMethod del = uut.RuleDelegateFromMethodName(customContainer, methodName);
            Assert.IsNotNull(del);

            Dictionary<string, object> contextPropertyBag = customContainer.PreparePropertiesForRule(methodName, null);
            contextPropertyBag.Add("ContextCompareObject", Guid.NewGuid());

            // No context object is passed. Should return false
            bool? isAuthorized = del.Invoke(PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsFalse(isAuthorized.Value);

            // A different context object is passed. Should return false
            isAuthorized = del.Invoke(PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles, Guid.NewGuid(), contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsFalse(isAuthorized.Value);

            // The same context object is passed. Should return true
            isAuthorized = del.Invoke(PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles, contextPropertyBag["ContextCompareObject"], contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void InvalidPropertyBagTest()
        {
            string methodName = "CustomRuleAreEqualRule";

            PermissionRules uut = new PermissionRules();
            ICustomRuleContainer customContainer = new TestCustomRules();
            PermissionRules.IsAuthorizedMethod del = uut.RuleDelegateFromMethodName(customContainer, methodName);
            Assert.IsNotNull(del);

            Dictionary<string, object> contextPropertyBag = customContainer.PreparePropertiesForRule(methodName, null);
            // contextPropertyBag.Add("ContextCompareObject", Guid.NewGuid()); // don't pass in a ContextCompareObject

            // No context compare object is passed. Should throw error
            bool? isAuthorized = del.Invoke(PermissionRulesTests.TestIdentity, PermissionRulesTests.TestRoles, null, contextPropertyBag);
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
