using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Open.SPF.Utility;
using Open.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Open.SPF.Core.Test
{
    [TestClass]
    public class ServiceRegistrationTests
    {

        [TestMethod]
        public void ServiceRegistrationInstantiationTest()
        {
            ServiceRegistration uut = new ServiceRegistration();

            object registeredComponent = uut.ServiceRegistrationInstance;

            Assert.IsNotNull(registeredComponent);
            Assert.IsInstanceOfType(registeredComponent, typeof(ServiceRegistration));
            Assert.IsInstanceOfType(registeredComponent, typeof(UnitTestServiceRegistration));
        }

        [ExpectedException(typeof(NotImplementedException))]
        [TestMethod]
        public void ServiceRegistrationFailNotImplementedTest()
        {
            ServiceRegistration uut = new ServiceRegistration(typeof(UnitTestFailServiceRegistration));
            uut.ConfigureServices(null);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void LoadServiceRegistrationFailEmptyStringTest()
        {
            ServiceRegistration uut = new ServiceRegistration("");

            object registeredComponent = uut.ServiceRegistrationInstance;
        }

        //[ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void LoadServiceRegistrationFailNullStringTest()
        {
            ServiceRegistration uut = new ServiceRegistration((string)null);

            object registeredComponent = uut.ServiceRegistrationInstance;

            //This method will no longer throw an error.
            //Instead, it will return the appsetings configured value
            Assert.IsNotNull(registeredComponent);
            Assert.IsInstanceOfType(registeredComponent, typeof(ServiceRegistration));
            Assert.IsInstanceOfType(registeredComponent, typeof(UnitTestServiceRegistration));
        }

        //[ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void LoadServiceRegistrationFailNullTypeTest()
        {
            ServiceRegistration uut = new ServiceRegistration((Type)null);

            object registeredComponent = uut.ServiceRegistrationInstance;

            //This method will no longer throw an error.
            //Instead, it will return the appsetings configured value
            Assert.IsNotNull(registeredComponent);
            Assert.IsInstanceOfType(registeredComponent, typeof(ServiceRegistration));
            Assert.IsInstanceOfType(registeredComponent, typeof(UnitTestServiceRegistration));
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void LoadServiceRegistrationFailInvalidStringTest()
        {
            ServiceRegistration uut = new ServiceRegistration("BOGUS");

            object registeredComponent = uut.ServiceRegistrationInstance;
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void LoadServiceRegistrationFailInvalidTypeTest()
        {
            ServiceRegistration uut = new ServiceRegistration(typeof(String));

            object registeredComponent = uut.ServiceRegistrationInstance;
        }

        [TestMethod]
        public void ServiceInstantiationTest()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            ServiceRegistration uut = new ServiceRegistration(typeof(UnitTestServiceRegistration));
            uut.ConfigureServices(serviceCollection);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            object item = serviceProvider.GetService<IUnitTestService>();

            Assert.IsNotNull(item);
            Assert.IsInstanceOfType(item, typeof(IUnitTestService));
            Assert.IsInstanceOfType(item, typeof(UnitTestServiceImpl));

            string result = ((IUnitTestService)item).UnitTestMethod();

            Assert.IsNotNull(result);
            Assert.AreEqual("SUCCESS", result);
        }

        [TestMethod]
        public void CompoundServiceInstantiationTest()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            ServiceRegistration uut = new ServiceRegistration(typeof(UnitTestServiceRegistration));
            uut.ConfigureServices(serviceCollection);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            object item = serviceProvider.GetService<ICompoundService>();

            Assert.IsNotNull(item);
            Assert.IsInstanceOfType(item, typeof(ICompoundService));
            Assert.IsInstanceOfType(item, typeof(CompoundServiceImpl));

            string result = ((ICompoundService)item).CompoundMethod();

            Assert.IsNotNull(result);
            Assert.AreEqual("SUCCESS", result);

            result = ((ICompoundService)item).InternalServiceName();

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(UnitTestServiceImpl).FullName, result);
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
