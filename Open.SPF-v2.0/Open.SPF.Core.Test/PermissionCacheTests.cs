using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using System.Security.Principal;

using Open.SPF.Permissions;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for PermissionCacheTests
    /// </summary>
    [TestClass]
    public class PermissionCacheTests
    {
        public PermissionCacheTests()
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
        public void PermissionCacheInstanceTest()
        {
            IPermissionCache uut = PermissionCache.Instance;

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(PermissionCache));
        }

        [TestMethod]
        public void DefaultPermissionCacheTest()
        {
            IPermissionCache uut = new PermissionCache();
            IPrincipal user = ClaimsPrincipal.Current;
            List<string> claims = new List<string>(new string[] { "FirstRole", "ThirdRole" });

            uut.SaveRolesForUser(user, claims);
            List<string> test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);
        }

        [TestMethod]
        public void TimedPermissionCacheNoTimeoutTest()
        {
            IPermissionCache uut = new TimedPermissionCache();
            IPrincipal user = ClaimsPrincipal.Current;
            List<string> claims = new List<string>(new string[] { "FirstRole", "ThirdRole" });

            uut.SaveRolesForUser(user, claims);
            List<string> test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);
        }
        
        //[TestMethod]
        public void TimedPermissionCacheSingleTimeoutTest()
        {
            IPermissionCache uut = new TimedPermissionCache();
            IPrincipal user = ClaimsPrincipal.Current;
            List<string> claims = new List<string>(new string[] { "FirstRole", "ThirdRole" });

            uut.SaveRolesForUser(user, claims);
            List<string> test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);

            System.Threading.Thread.Sleep(70000);
            test = uut.GetRolesForUser(user);
            Assert.IsNull(test);
        }

        //[TestMethod]
        public void TimedPermissionCacheRollingTimeoutTest()
        {
            IPermissionCache uut = new TimedPermissionCache();
            IPrincipal user = ClaimsPrincipal.Current;
            List<string> claims = new List<string>(new string[] { "FirstRole", "ThirdRole" });

            uut.SaveRolesForUser(user, claims);
            List<string> test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);

            System.Threading.Thread.Sleep(40000);
            test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);

            System.Threading.Thread.Sleep(40000);
            test = uut.GetRolesForUser(user);

            Assert.IsNotNull(test);
            Assert.AreEqual(2, test.Count);
            Assert.AreEqual("FirstRole", test[0]);
            Assert.AreEqual("ThirdRole", test[1]);

            System.Threading.Thread.Sleep(70000);
            test = uut.GetRolesForUser(user);
            Assert.IsNull(test);
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
        [TestInitialize()]
        public void RunBeforeEachTest()
        {
            System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity("unittest\\user", "UnitTestAuth");

            System.Security.Principal.GenericPrincipal gp = new System.Security.Principal.GenericPrincipal(identity, new string[] { "FirstRole", "ThirdRole" });

            System.Threading.Thread.CurrentPrincipal = gp;
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void RunAfterEachTest() { }
        //
        #endregion
    }
}
