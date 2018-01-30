using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Open.SPF.Core;
using Open.SPF.Web;

namespace Open.SPF.Web.Test
{
    /// <summary>
    /// Summary description for MembershipPermissionManagerTests
    /// </summary>
    [TestClass]
    public class MembershipPermissionManagerTests
    {
        public MembershipPermissionManagerTests()
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
        public void DefaultDoGetRolesForUserTest()
        {
            PermissionManager uut = new MembershipPermissionManager();
            Assert.IsNotNull(uut);
            uut.Initialze();
            List<string> userRoles = uut.DoGetRolesForUser(System.Threading.Thread.CurrentPrincipal);

            Assert.IsNotNull(userRoles);
            Assert.IsTrue(userRoles.Count > 0);
        }


        [TestMethod]
        public void PermissionManagerInstanceTest()
        {
            IPermissionManager uut = PermissionManager.Instance;

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(MembershipPermissionManager));
        }

        [TestMethod]
        public void CurrentUserTest()
        {
            IPermissionManager uut = PermissionManager.Instance;
            Assert.IsNotNull(uut);
            System.Security.Principal.IPrincipal currentUser = uut.CurrentUser;

            Assert.IsNotNull(currentUser);
            Assert.IsNotNull(currentUser.Identity);
            Assert.AreEqual("unittest\\user", currentUser.Identity.Name);
        }

        [TestMethod]
        public void GetUserRolesTest()
        {
            IPermissionManager uut = PermissionManager.Instance;
            Assert.IsNotNull(uut);
            List<string> userRoles = uut.GetUserRoles(System.Threading.Thread.CurrentPrincipal);

            Assert.IsNotNull(userRoles);
            Assert.IsTrue(userRoles.Count > 0);
        }

        [TestMethod]
        public void AssertPermissionTest()
        {
            bool test = PermissionManager.AssertPermission("FirstPermission");
            Assert.IsTrue(test);

            test = PermissionManager.AssertPermission("SecondPermission");
            Assert.IsFalse(test);

            test = PermissionManager.AssertPermission("ThirdPermission");
            Assert.IsTrue(test);
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
