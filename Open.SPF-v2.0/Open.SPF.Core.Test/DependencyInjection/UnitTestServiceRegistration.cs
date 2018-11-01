//using Open.SPF.DependencyInjection;
//using Open.SPF.Utility;
//using Open.SPF.Utility.Dev;
using Open.SPF.Permissions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core.Test
{
    public class UnitTestServiceRegistration : Open.Common.DependencyInjection.ServiceRegistration
    {
        protected override void DoConfigureServices(IServiceCollection services)
        {
            // For service registration tests only
            services.AddTransient<IUnitTestService, UnitTestServiceImpl>();
            services.AddTransient<ICompoundService, CompoundServiceImpl>();

            // For testing of other core services
            //services.AddTransient<IKnowSecrets, ConstantValueSecretManager>();
            services.AddTransient<IPermissionCache, PermissionCache>();
            services.AddTransient<IPermissionManager, PermissionManager>();
        }
    }
}
