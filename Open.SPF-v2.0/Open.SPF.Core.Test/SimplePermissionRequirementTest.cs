using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using System.Collections.Generic;
using Open.SPF.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;

namespace Open.SPF.Core.Test
{
    [TestClass]
    public class SimplePermissionRequirementTest
    {
        [TestMethod]
        public void HandleRequirementSucceedTest()
        {
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity("UnitTestUser"), new List<Claim> { new Claim(ClaimTypes.Role, "SecondRole") }));
            IAuthorizationRequirement requirement = new SimplePermissionRequirement("SecondPermission");

            AuthorizationHandlerContext authorizationContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, null);

            AuthorizationHandler<SimplePermissionRequirement> authorizationHandler = (AuthorizationHandler<SimplePermissionRequirement>)requirement;
            authorizationHandler.HandleAsync(authorizationContext);

            Assert.IsTrue(authorizationContext.HasSucceeded);
        }

        [TestMethod]
        public void HandleRequirementFailTest()
        {
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity("UnitTestUser"), new List<Claim> { new Claim(ClaimTypes.Role, "SecondRole") }));
            IAuthorizationRequirement requirement = new SimplePermissionRequirement("FirstPermission");

            AuthorizationHandlerContext authorizationContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, null);

            AuthorizationHandler<SimplePermissionRequirement> authorizationHandler = (AuthorizationHandler<SimplePermissionRequirement>)requirement;
            authorizationHandler.HandleAsync(authorizationContext);

            Assert.IsFalse(authorizationContext.HasSucceeded);
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
        //[TestInitialize()]
        //public void RunBeforeEachTest()
        //{
        //    System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity("unittest\\user", "UnitTestAuth");

        //    System.Security.Principal.GenericPrincipal gp = new System.Security.Principal.GenericPrincipal(identity, new string[] { "FirstRole", "ThirdRole" });

        //    System.Threading.Thread.CurrentPrincipal = gp;
        //}

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void RunAfterEachTest() { }
        //
        #endregion
    }
}
