using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.SPF.Core;
using System.Web;
using System.IO;
using Owin;
using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace Open.SPF.Web.Test
{
    /// <summary>
    /// Summary description for OwinPermissionManagerTests
    /// </summary>
    [TestClass]
    public class OwinPermissionManagerTests
    {
        private static IDisposable _appHandle;

        public OwinPermissionManagerTests()
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
        public void CurrentUserTest()
        {
            IPermissionManager uut = new OwinPermissionManager();
            System.Security.Principal.IPrincipal currentUser = uut.CurrentUser;

            Assert.IsNotNull(currentUser);
            Assert.IsNotNull(currentUser.Identity);
            Assert.AreEqual("unittest\\user", currentUser.Identity.Name);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void RunBeforeAllTests(TestContext testContext)
        {
            string baseAddress = "http://localhost:9876/";
            _appHandle = WebApp.Start<Open.SPF.Web.Test.Owin.Startup>(url: baseAddress);

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.Timeout = new TimeSpan(0, 10, 0);

            HttpResponseMessage response = client.GetAsync(baseAddress + "api/values").Result; 

            if (response.StatusCode == System.Net.HttpStatusCode.Found)
            {
                HttpResponseMessage newResponse;
                if ((response.Headers.Location != null))
                {
                    newResponse = client.GetAsync(response.Headers.Location.ToString()).Result;
                }
                else
                {
                    newResponse = client.GetAsync(baseAddress + "api/values").Result;
                }
            }
            //HttpContext.Current = new HttpContext(
            //    new HttpRequest("", "http://tempuri.org", ""),
            //    new HttpResponse(new StringWriter())
            //    );
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void RunAfterAllTests() 
        {
            _appHandle.Dispose();
            _appHandle = null;
        }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void RunBeforeEachTest()
        {
            //System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity("unittest\\user", "UnitTestAuth");

            //System.Security.Principal.GenericPrincipal gp = new System.Security.Principal.GenericPrincipal(identity, new string[] { "FirstRole", "ThirdRole" });

            //System.Threading.Thread.CurrentPrincipal = gp;

            //HttpContext.Current = new HttpContext(
            //    new HttpRequest("", "http://tempuri.org", ""),
            //    new HttpResponse(new StringWriter())
            //    );

        }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void RunAfterEachTest() { }
        //
        #endregion
    }
}
