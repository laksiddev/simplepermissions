using Open.SPF.Utility;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public static class MockAuthenticationExtensions
    {
        public static IAppBuilder UseMockAuthentication(this IAppBuilder app, MockAuthenticationOptions options)
        {
            TraceUtility.WriteTrace(typeof(MockAuthenticationExtensions), "UseMockAuthentication(IAppBuilder, MockAuthenticationOptions)", TraceUtility.TraceType.Watch);
            return app.Use(typeof(MockAuthenticationMiddleware), app, options);
        }
    }
}
