using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Open.SPF.Utility;
using Open.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using CommonServiceLocator;

namespace Open.SPF.Core.Test
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void ServiceLocatorInialize1Test()
        {
            IServiceLocator uut = DependencyInjectionServiceLocator.Initialize();

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(DependencyInjectionServiceLocator));
        }

        [TestMethod]
        public void ServiceLocatorInialize2Test()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceLocator uut = DependencyInjectionServiceLocator.Initialize(serviceCollection);

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(DependencyInjectionServiceLocator));
        }

        [TestMethod]
        public void ServiceInstantiationTest()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            IServiceLocator uut = DependencyInjectionServiceLocator.Initialize(serviceCollection);

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(DependencyInjectionServiceLocator));

            object item = uut.GetInstance<IUnitTestService>();

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

            IServiceLocator uut = DependencyInjectionServiceLocator.Initialize(serviceCollection);

            Assert.IsNotNull(uut);
            Assert.IsInstanceOfType(uut, typeof(DependencyInjectionServiceLocator));

            object item = uut.GetInstance<ICompoundService>();

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
