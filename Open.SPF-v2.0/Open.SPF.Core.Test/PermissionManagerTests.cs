using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

using Open.SPF.Windows;
using Open.SPF.Permissions;
using System.Diagnostics;

namespace Open.SPF.Core.Test
{
    /// <summary>
    /// Summary description for PermissionManagerTests
    /// </summary>
    [TestClass]
    public class PermissionManagerTests
    {
        public PermissionManagerTests()
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
        public void PermissionManagerInstanceTest()
        {
            IPermissionManager uut = PermissionManager.Instance;

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(PermissionManager));
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

            test = PermissionManager.AssertPermission("ForthPermission");
            Assert.IsTrue(test);

            test = PermissionManager.AssertPermission("FifthPermission");
            Assert.IsFalse(test);
        }

        [TestMethod]
        public void DefaultDoGetRolesForUserTest()
        {
            PermissionManager uut = new PermissionManager();
            Assert.IsNotNull(uut);
            uut.Initialze();
            List<string> userRoles = uut.DoGetRolesForUser(System.Threading.Thread.CurrentPrincipal);

            Assert.IsNotNull(userRoles);
            Assert.IsTrue(userRoles.Count > 0);
        }

        //[TestMethod]
        public void ADDoGetRolesForUserTest()
        {
            PermissionManager uut = new ActiveDirectoryPermissionManager();
            Assert.IsNotNull(uut);
            uut.Initialze();
            List<string> userRoles = uut.DoGetRolesForUser(System.Threading.Thread.CurrentPrincipal);

            Assert.IsNotNull(userRoles);
            Assert.IsTrue(userRoles.Count > 0);
        }

        [TestMethod]
        public void AssertCustomPermissionsTest()
        {
            bool test = PermissionManager.AssertPermission("CheckArgument1");
            Assert.IsTrue(test);

            test = PermissionManager.AssertPermission("CheckArgument2");
            Assert.IsTrue(test);

            test = PermissionManager.AssertPermission("CheckArgument3");
            Assert.IsTrue(test);
        }
        

       [TestMethod]
        public void AssertCustomPermissionCustomArgumentTest()
        {
            Dictionary<string, object> propertyBag = new Dictionary<string, object>();
            propertyBag.Add("CustomArgument", "CustomValue");

            bool test = PermissionManager.AssertPermission("CheckCustomArgument", null, propertyBag);
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void AssertCustomPermissionCustomObjectTest()
        {
            StringBuilder customObject = new StringBuilder();
            customObject.Append("CustomValue");

            bool test = PermissionManager.AssertPermission("CheckCustomObject", customObject, null);
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void AssertRecursivePermissionCheckTest()
        {
            // We may want to test core permissions from within a custom permission
            bool test = PermissionManager.AssertPermission("RecursivePermissionCheck");
            Assert.IsTrue(test);
        }

        [TestMethod]
        public void ConfigureServicesTest()
        {
            PermissionManager uut = new PermissionManager();
            Assert.IsNotNull(uut);
            uut.Initialze();

            ServiceCollection serviceCollection = new ServiceCollection();
            uut.ConfigureServices(serviceCollection);

            foreach (Microsoft.Extensions.DependencyInjection.ServiceDescriptor item in serviceCollection)
            {
                if (item.ImplementationInstance is Microsoft.Extensions.Options.ConfigureNamedOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>)
                {
                    Microsoft.Extensions.Options.ConfigureNamedOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions> options =
                        (Microsoft.Extensions.Options.ConfigureNamedOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>)item.ImplementationInstance;

                    // It's almost impossible to see if the services were configured correctly, but if the method is manually debugged, you can see information in 
                    // options.Action.Target.permissionName
                }
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void RunBeforeAllTests(TestContext testContext) 
        //{
        //}

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
