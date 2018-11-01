using System;
using System.Collections.Generic;
using System.Configuration;


using Open.SPF.Utility;
using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Open.SPF.DependencyInjection
{
    public class PermissionServiceLocator : ServiceLocatorImplBase
    {
        private IServiceProvider _container;
        private static PermissionServiceLocator _instance;

        public PermissionServiceLocator(IServiceProvider container)
        {
            _container = container;
        }

        public static PermissionServiceLocator Initialize()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            return Initialize(serviceCollection);
        }

        public static PermissionServiceLocator Initialize(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException("Services", "A Service Collection is required for initialization.");

            TraceUtility.WriteInformationTrace(typeof(PermissionServiceLocator), "Initialze", Open.SPF.Utility.TraceUtility.TraceType.Begin);

            ServiceRegistration serviceRegistration = new ServiceRegistration();
            serviceRegistration.ConfigureServices(services);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            PermissionServiceLocator permissionServiceLocator = new PermissionServiceLocator(serviceProvider);

            services.AddSingleton(typeof(IServiceLocator), permissionServiceLocator);

            object test = permissionServiceLocator.GetInstance<IServiceLocator>();
            if (test == null)
            {
                TraceUtility.LogWarningMessage("Unity configuration did not contain an element for IServiceLocator.");
            }

            //test = instance.GetInstance<Open.SPF.Core.IPermissionCache>();
            //if (test == null)
            //{
            //    Open.SPF.Utility.EventLogUtility.LogWarningMessage("Unity configuration did not contain an element for IPermissionCache. Registering a default instance instead.");
            //    container.RegisterInstance(typeof(IPermissionCache), new PermissionCache(), new PerThreadLifetimeManager());
            //}

            //Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "Initialze", null, String.Format("Locator instance {0} NULL.", ((instance != null) ? "is NOT" : "is")), Open.SPF.Utility.TraceUtility.TraceType.End);
            //return instance;
            _instance = permissionServiceLocator;

            return permissionServiceLocator;
        }

        public static IServiceLocator LocatorInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = PermissionServiceLocator.Initialize();
                }

                return _instance;
            }
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            try
            {
               TraceUtility.WriteInformationTrace(typeof(PermissionServiceLocator), "DoGetInstance(Type, string)", "_container.Resolve(serviceType, key)", null, Open.SPF.Utility.TraceUtility.TraceType.Begin);
                object instance = _container.GetService(serviceType);
                TraceUtility.WriteInformationTrace(typeof(PermissionServiceLocator), "DoGetInstance(Type, string)", "_container.Resolve(serviceType, key)", null, Open.SPF.Utility.TraceUtility.TraceType.End);
                return instance;
            }
            catch (Exception ex) // { /* ignore - return null */ }
            {
                Open.SPF.Utility.TraceUtility.LogWarningMessage(String.Format("An error occurred while attempting to find the requested instance of type: {0}. \r\n\r\n{1}", serviceType.FullName, Open.SPF.Utility.TraceUtility.FormatExceptionMessage(ex)));
            }

            return null;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            try
            {
                TraceUtility.WriteInformationTrace(typeof(PermissionServiceLocator), "DoGetAllInstances(Type)", "_container.ResolveAll(serviceType)", null, Open.SPF.Utility.TraceUtility.TraceType.Begin);
                IEnumerable<object> instances = _container.GetServices(serviceType);
                TraceUtility.WriteInformationTrace(typeof(PermissionServiceLocator), "DoGetAllInstances(Type)", "_container.ResolveAll(serviceType)", null, Open.SPF.Utility.TraceUtility.TraceType.End);
                return instances;
            }
            catch (Exception ex) // { /* ignore - return null */ }
            {
                Open.SPF.Utility.TraceUtility.LogWarningMessage(String.Format("An error occurred while attempting to find the requested instance of type: {0}. \r\n\r\n{1}", serviceType.FullName, Open.SPF.Utility.TraceUtility.FormatExceptionMessage(ex)));
            }

            return null;
        }
    }
}
