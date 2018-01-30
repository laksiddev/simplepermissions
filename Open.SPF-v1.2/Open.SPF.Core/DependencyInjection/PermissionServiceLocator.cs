using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.ServiceLocation;

using Open.SPF.Core;

namespace Open.SPF.DependencyInjection
{
    public class PermissionServiceLocator : ServiceLocatorImplBase
    {
        private IUnityContainer _container;

        public PermissionServiceLocator(IUnityContainer container)
        {
            _container = container;
         }

        public static PermissionServiceLocator Initialize()
        {
            Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "Initialze", Open.SPF.Utility.TraceUtility.TraceType.Begin);
            IUnityContainer container = new UnityContainer();
            PermissionServiceLocator instance = new PermissionServiceLocator(container);

            container.RegisterInstance<IServiceLocator>(instance);
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(instance.GetServiceLocatorInstance));

            UnityConfigurationSection section = null;
            try
            {
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "Initialze", "section.Containers[0].Configure(container)", null, Open.SPF.Utility.TraceUtility.TraceType.Begin);
                section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                if ((section != null) && (section.Containers.Count > 0))
                {
                    section.Configure(container, "defaultContainer");
                }
                else
                {
                    Open.SPF.Utility.EventLogUtility.LogWarningMessage("The Unity configuration section was not found or was not properly defined.");
                }
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "Initialze", "section.Containers[0].Configure(container)", null, Open.SPF.Utility.TraceUtility.TraceType.End);
            }
            catch (Exception ex)
            {
                Open.SPF.Utility.EventLogUtility.LogWarningMessage(String.Format("There was an error reading the Unity configuration section:\r\n\r\n{0}", Open.SPF.Utility.EventLogUtility.FormatExceptionMessage(ex)));
            }

            object test = instance.GetInstance<Open.SPF.Core.IPermissionManager>();
            if (test == null)
            {
                Open.SPF.Utility.EventLogUtility.LogWarningMessage("Unity configuration did not contain an element for IPermissionManager. Registering a default instance instead.");
                container.RegisterInstance(typeof(IPermissionManager), new PermissionManager(), new PerThreadLifetimeManager());
            }

            test = instance.GetInstance<Open.SPF.Core.IPermissionCache>();
            if (test == null)
            {
                Open.SPF.Utility.EventLogUtility.LogWarningMessage("Unity configuration did not contain an element for IPermissionCache. Registering a default instance instead.");
                container.RegisterInstance(typeof(IPermissionCache), new PermissionCache(), new PerThreadLifetimeManager());
            }

            Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "Initialze", null, String.Format("Locator instance {0} NULL.", ((instance != null) ? "is NOT" : "is")), Open.SPF.Utility.TraceUtility.TraceType.End);
            return instance;
        }

        public IServiceLocator GetServiceLocatorInstance()
        {
            return this;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            try
            {
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "DoGetInstance(Type, string)", "_container.Resolve(serviceType, key)", null, Open.SPF.Utility.TraceUtility.TraceType.Begin);
                object instance = _container.Resolve(serviceType, key);
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "DoGetInstance(Type, string)", "_container.Resolve(serviceType, key)", null, Open.SPF.Utility.TraceUtility.TraceType.End);
                return instance;
            }
            catch (Microsoft.Practices.Unity.ResolutionFailedException ex) // { /* ignore - return null */ }
            { 
                Open.SPF.Utility.EventLogUtility.LogWarningMessage(String.Format("An error occurred while attempting to find the requested instance of type: {0}. \r\n\r\n{1}", serviceType.FullName, Open.SPF.Utility.EventLogUtility.FormatExceptionMessage(ex)));
            }

            return null;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            try
            {
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "DoGetAllInstances(Type)", "_container.ResolveAll(serviceType)", null, Open.SPF.Utility.TraceUtility.TraceType.Begin);
                IEnumerable<object> instances = _container.ResolveAll(serviceType);
                Open.SPF.Utility.TraceUtility.WriteTrace(typeof(PermissionServiceLocator), "DoGetAllInstances(Type)", "_container.ResolveAll(serviceType)", null, Open.SPF.Utility.TraceUtility.TraceType.End);
                return instances;
            }
            catch (Microsoft.Practices.Unity.ResolutionFailedException ex) // { /* ignore - return null */ }
            {
                Open.SPF.Utility.EventLogUtility.LogWarningMessage(String.Format("An error occurred while attempting to find the requested instance of type: {0}. \r\n\r\n{1}", serviceType.FullName, Open.SPF.Utility.EventLogUtility.FormatExceptionMessage(ex)));
            }

            return null;
        }
    }
}
