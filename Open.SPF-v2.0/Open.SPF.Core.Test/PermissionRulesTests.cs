using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Reflection;

using Open.SPF.Permissions;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for PermissionRulesTests
    /// </summary>
    [TestClass]
    public class PermissionRulesTests
    {
        public PermissionRulesTests()
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
        public void AlwaysAuthorizedRuleTest()
        {
            TestCustomRules uut = new TestCustomRules();
            PermissionRules.IsAuthorizedMethod del = new PermissionRules.IsAuthorizedMethod(uut.CustomRuleIsAlwaysAuthorized);

            Dictionary<string, object> contextPropertyBag = uut.PreparePropertiesForRule("CustomRuleIsAlwaysAuthorized", null);

            bool? isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);
        }

        [TestMethod]
        public void NeverAuthorizedRuleTest()
        {
            TestCustomRules uut = new TestCustomRules();
            PermissionRules.IsAuthorizedMethod del = new PermissionRules.IsAuthorizedMethod(uut.CustomRuleIsNeverAuthorized);

            Dictionary<string, object> contextPropertyBag = uut.PreparePropertiesForRule("CustomRuleIsNeverAuthorized", null);

            bool? isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsFalse(isAuthorized.Value);
        }

        [TestMethod]
        public void ParallelRulesTest()
        {
            List<Task<bool?>> ruleTasks = new List<Task<bool?>>();
            TestCustomRules uut = new TestCustomRules();

            PermissionRules.IsAuthorizedMethod del1 = new PermissionRules.IsAuthorizedMethod(uut.CustomRuleIsAlwaysAuthorized);
            Dictionary<string, object> contextPropertyBag1 = uut.PreparePropertiesForRule("CustomRuleIsAlwaysAuthorized", null);

            PermissionRules.IsAuthorizedMethod del2 = new PermissionRules.IsAuthorizedMethod(uut.CustomRuleIsNeverAuthorized);
            Dictionary<string, object> contextPropertyBag2 = uut.PreparePropertiesForRule("CustomRuleIsNeverAuthorized", null);

            Guid alwaysRuleGuid = Guid.NewGuid();
            Task<bool?> task1 = new Task<bool?>(a => del1.Invoke(TestIdentity, TestRoles, null, contextPropertyBag1), alwaysRuleGuid);
            task1.Start();
            ruleTasks.Add(task1);

            Guid neverRuleGuid = Guid.NewGuid();
            Task<bool?> task2 = new Task<bool?>(a => del2.Invoke(TestIdentity, TestRoles, null, contextPropertyBag2), neverRuleGuid);
            task2.Start();
            ruleTasks.Add(task2);

            // It's almost pointless because both tasks have likely finished by this point, but it's worth excercising the proper code
            Task.WaitAll(ruleTasks.ToArray());

            bool wasAlwaysRuleFound = false;
            bool wasNeverRuleFound = false;
            foreach(Task<bool?> ruleTask in ruleTasks)
            {
                if ((Guid)ruleTask.AsyncState == alwaysRuleGuid)
                {
                    Assert.IsTrue(ruleTask.Result.HasValue);
                    Assert.IsTrue(ruleTask.Result.Value);
                    wasAlwaysRuleFound = true;
                }
                else if ((Guid)ruleTask.AsyncState == neverRuleGuid)
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
            string typeDefinition = "Open.SPF.Core.Test.TestCustomRules, Open.SPF.Core.Test, Version=2.0, Culture=neutral, PublicKeyToken=b4313207536550be";
            string methodName = "CustomRuleIsAlwaysAuthorized";

            Type ruleType = Type.GetType(typeDefinition);
            Assert.IsTrue(typeof(ICustomRuleContainer).IsAssignableFrom(ruleType));
            ConstructorInfo constructorMethod = ruleType.GetConstructor(new Type[0]);
            Assert.IsNotNull(constructorMethod);

            ICustomRuleContainer uut; // = new TestCustomRules();
            uut = (ICustomRuleContainer)constructorMethod.Invoke(new object[0]);
            Assert.IsNotNull(uut);

            Type[] methodSignature = new Type[] { typeof(IIdentity), typeof(IEnumerable<string>), typeof(object), typeof(Dictionary<string, object>)};
            MethodInfo ruleMethodInfo = ruleType.GetMethod(methodName, methodSignature);
            Assert.IsNotNull(ruleMethodInfo);

            PermissionRules.IsAuthorizedMethod del = (PermissionRules.IsAuthorizedMethod)ruleMethodInfo.CreateDelegate(typeof(PermissionRules.IsAuthorizedMethod), uut);
            Assert.IsNotNull(del);

            Dictionary<string, object> contextPropertyBag = uut.PreparePropertiesForRule("CustomRuleIsAlwaysAuthorized", null);

            bool? isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);
        }

        [TestMethod]
        public void RuleFromInvalidRuleTest()
        {
            ICustomRuleContainer testContainer = new TestCustomRules();

            PermissionRules uut = new PermissionRules();
            PermissionRules.IsAuthorizedMethod testRule = uut.RuleDelegateFromMethodName(testContainer, "BOGUS");
            Assert.IsNull(testRule);

            testRule = uut.RuleDelegateFromMethodName(testContainer, "CustomRuleIsNotValidRule");
            Assert.IsNull(testRule);


            testRule = uut.RuleDelegateFromMethodName(testContainer, "CustomRuleWrongSignatureRule");
            Assert.IsNull(testRule);
        }

        [TestMethod]
        public void CoreUnrestrictedRuleTest()
        {
            PermissionRules uut = new PermissionRules();
            PermissionRules.IsAuthorizedMethod del = new PermissionRules.IsAuthorizedMethod(uut.IsUnrestrictedRule);

            Dictionary<string, object> contextPropertyBag = uut.PrepareStateForUnrestrictedRule(true);

            bool? isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);

            contextPropertyBag = uut.PrepareStateForUnrestrictedRule(false);

            isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsFalse(isAuthorized.Value);
        }

        [TestMethod]
        public void CoreUserRoledRuleTest()
        {
            PermissionRules uut = new PermissionRules();
            PermissionRules.IsAuthorizedMethod del = new PermissionRules.IsAuthorizedMethod(uut.IsUserRoleAuthorizedRule);

            Dictionary<string, object> contextPropertyBag = uut.PrepareStateForRoleRule(TestRoles[0]);

            bool? isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsTrue(isAuthorized.Value);

            contextPropertyBag = uut.PrepareStateForRoleRule("BOGUS");

            isAuthorized = del.Invoke(TestIdentity, TestRoles, null, contextPropertyBag);
            Assert.IsTrue(isAuthorized.HasValue);
            Assert.IsFalse(isAuthorized.Value);
        }

        [TestMethod]
        public void ReadRulesFromConfigurationTest()
        {
            PermissionRules uut = new PermissionRules();
            Dictionary<string, PermissionRuleContextCollection> contextLookup = uut.ReadRulesFromConfiguration();
            Assert.IsNotNull(contextLookup);
            Assert.AreEqual(11, contextLookup.Keys.Count);

            int iCount = 0;
            foreach(string permissionName in contextLookup.Keys)
            {
                iCount += contextLookup[permissionName].Count;
            }
            Assert.AreEqual(12, iCount);
        }

        [TestMethod]
        public void RuleNameTests()
        {
            string ruleWithNoCustomName = "CustomRuleIsAlwaysAuthorized";
            string ruleWithCustomName = "CustomRuleAreEqualRule";

            ICustomRuleContainer customContainer = new TestCustomRules();
            PermissionRules uut = new PermissionRules();

            string ruleName = uut.RuleNameFromMethodName(customContainer, ruleWithNoCustomName);
            Assert.IsNotNull(ruleName);
            Assert.AreEqual(ruleWithNoCustomName, ruleName);

            ruleName = uut.RuleNameFromMethodName(customContainer, ruleWithCustomName);
            Assert.IsNotNull(ruleName);
            Assert.AreEqual(ruleWithCustomName + "-CustomName", ruleName);
        }

        [TestMethod]
        public void RuleArgumentTests()
        {
            string ruleWithArgument = "CheckArgument1";

            PermissionRules uut = new PermissionRules();
            Dictionary<string, PermissionRuleContextCollection> contextLookup = uut.ReadRulesFromConfiguration();
            Assert.IsNotNull(contextLookup);

            Assert.IsTrue(contextLookup.ContainsKey(ruleWithArgument));
            PermissionRuleContextCollection ruleContextCollection = contextLookup[ruleWithArgument];
            Assert.IsNotNull(ruleContextCollection);
            foreach(PermissionRuleContext rule in ruleContextCollection.Items)
            {
                foreach(string key in rule.PropertyBag.Keys)
                {
                    if (rule.PropertyBag[key] is string)
                    {
                        Console.WriteLine(String.Format("Key: {0}, Value: {1}", key, (String)rule.PropertyBag[key]));
                    }
                }
            }
        } 

        public static IIdentity TestIdentity
        {
            get { return new GenericIdentity("TestUser");  }
        }

        public static List<string> TestRoles
        {
            get 
            {
                List<string> testRoles = new List<string>();
                testRoles.Add("FirstRole");
                testRoles.Add("ThirdRole");

                return testRoles;
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
