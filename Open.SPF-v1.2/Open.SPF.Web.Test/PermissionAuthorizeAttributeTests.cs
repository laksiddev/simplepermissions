using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Open.SPF.Web.Test
{
    /// <summary>
    /// Summary description for PermissionAuthorizeAttributeTests
    /// </summary>
    [TestClass]
    public class PermissionAuthorizeAttributeTests
    {
        public PermissionAuthorizeAttributeTests()
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
        public void MvcAuthorizeAttributeTest()
        {
            System.Web.Mvc.AuthorizationContext authContext = MockAuthorizationContext;

            Open.SPF.Web.Mvc.PermissionAuthorizeAttribute uut = new Open.SPF.Web.Mvc.PermissionAuthorizeAttribute("FirstPermission, ThirdPermission");
            uut.OnAuthorization(authContext);
            Assert.IsNull(authContext.Result);

            uut = new Open.SPF.Web.Mvc.PermissionAuthorizeAttribute("FirstPermission");
            uut.OnAuthorization(authContext);
            Assert.IsNull(authContext.Result);

            uut = new Open.SPF.Web.Mvc.PermissionAuthorizeAttribute("BOGUS");
            uut.OnAuthorization(authContext);
            Assert.IsNotNull(authContext.Result);
            Assert.IsInstanceOfType(authContext.Result, typeof(System.Web.Mvc.RedirectResult));
            Assert.AreEqual(Properties.Settings.Default.UmauthorizedRedirectPage, ((System.Web.Mvc.RedirectResult)authContext.Result).Url);
        }

        [TestMethod]
        public void ApiAuthorizeAttributeTest()
        {
            System.Web.Http.Controllers.HttpActionContext apiContext = MockActionContext;

            System.Web.Http.AuthorizeAttribute uut = new Open.SPF.Web.Api.PermissionAuthorizeAttribute("FirstPermission, ThirdPermission");
            uut.OnAuthorization(apiContext);
            Assert.IsNull(apiContext.Response);

            uut = new Open.SPF.Web.Api.PermissionAuthorizeAttribute("BOGUS");
            uut.OnAuthorization(apiContext);
            Assert.IsNotNull(apiContext.Response);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, apiContext.Response.StatusCode);
        }

        private System.Web.Http.Controllers.HttpControllerContext CreateControllerContext()
        {
            System.Web.Http.HttpConfiguration config = new System.Web.Http.HttpConfiguration();
            System.Web.Http.Routing.IHttpRouteData route = new System.Web.Http.Routing.HttpRouteData(new System.Web.Http.Routing.HttpRoute());
            System.Net.Http.HttpRequestMessage req = new System.Net.Http.HttpRequestMessage();
            req.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey] = config;
            req.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpRouteDataKey] = route;

            System.Web.Http.Controllers.HttpControllerContext context = new System.Web.Http.Controllers.HttpControllerContext(config, route, req);
            context.ControllerDescriptor = new System.Web.Http.Controllers.HttpControllerDescriptor(config, "MockController", typeof(System.Web.Http.ApiController)); 

            return context;
        }

        private System.Web.Http.Controllers.HttpActionContext MockActionContext
        {
            get
            {
                System.Web.Http.Controllers.HttpControllerContext context = CreateControllerContext();
                System.Web.Http.Controllers.HttpActionDescriptor descriptor = new Mock<System.Web.Http.Controllers.HttpActionDescriptor>() { CallBase = true }.Object;
                return new System.Web.Http.Controllers.HttpActionContext(context, descriptor);
            }
        }

        private System.Web.Mvc.AuthorizationContext MockAuthorizationContext
        {
            get
            {
                Mock<System.Web.HttpRequestBase> mockRequest = new Mock<System.Web.HttpRequestBase>() { CallBase = true };
                mockRequest.SetupGet(r => r.HttpMethod).Returns("GET");
                Mock<System.Web.HttpContextBase> mockHttpContext = new Mock<System.Web.HttpContextBase>() { CallBase = true };
                mockHttpContext.SetupGet(c => c.Items).Returns(new Dictionary<object, object>());

                mockHttpContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
                Mock<System.Web.HttpSessionStateBase> mockSession = new Mock<System.Web.HttpSessionStateBase>() { CallBase = true };
                mockHttpContext.SetupGet(x => x.Session).Returns(mockSession.Object);

                Mock<System.Web.HttpResponseBase> mockResponse = new Mock<System.Web.HttpResponseBase>() { CallBase = true };
                Mock<System.Web.HttpCachePolicyBase> mockCache = new Mock<System.Web.HttpCachePolicyBase>();
                mockResponse.SetupGet(x => x.Cache).Returns(mockCache.Object);
                mockHttpContext.SetupGet(c => c.Response).Returns(mockResponse.Object);

                Mock<System.Web.Mvc.ControllerBase> mockController = new Mock<System.Web.Mvc.ControllerBase>();
                System.Web.Mvc.ControllerBase controller = mockController.Object;
                System.Web.Mvc.ControllerContext controllerContext = 
                    new System.Web.Mvc.ControllerContext(mockHttpContext.Object, new System.Web.Routing.RouteData(), controller);
                controller.ControllerContext = controllerContext;

                Mock<System.Web.Mvc.ControllerDescriptor> mockControllerDescriptor = new Mock<System.Web.Mvc.ControllerDescriptor>();
                Mock <System.Web.Mvc.ActionDescriptor>  mockActionDescriptor = new Mock<System.Web.Mvc.ActionDescriptor>();
                mockActionDescriptor.SetupGet(a => a.ActionName).Returns("Index");
                mockActionDescriptor.SetupGet(a => a.ControllerDescriptor).Returns(mockControllerDescriptor.Object);

                System.Web.Mvc.AuthorizationContext authContext = new System.Web.Mvc.AuthorizationContext(controllerContext, mockActionDescriptor.Object);

                return authContext;
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
        [TestInitialize()]
        public void RunBeforeEachTest()
        {
            System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity("unittest\\user", "UnitTestAuth");

            System.Security.Principal.GenericPrincipal gp = new System.Security.Principal.GenericPrincipal(identity, new string[] { "FirstRole", "ThirdRole" });

            System.Threading.Thread.CurrentPrincipal = gp;
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void RunAfterEachTest() { }
        //
        #endregion
    }
}
