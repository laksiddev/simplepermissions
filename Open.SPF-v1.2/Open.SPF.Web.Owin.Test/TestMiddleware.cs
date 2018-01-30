using Microsoft.Owin;
using Open.SPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public class TestMiddleware : OwinMiddleware
    {
        public TestMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            string detail = String.Format("context.Request.User.Identity.Name: {0}, context.Authentication.User.Identity.Name: {1}, context.Authentication.AuthenticationResponseGrant.Identity.Name: {2}", 
                (((context.Request != null) && (context.Request.User != null)) ? context.Request.User.Identity.Name : "NULL"), 
                (((context.Authentication.User != null) && (context.Authentication.User.Identity != null)) ? context.Authentication.User.Identity.Name : "NULL"),
                (((context.Authentication.AuthenticationResponseGrant != null) && (context.Authentication.AuthenticationResponseGrant.Identity != null)) ? context.Authentication.AuthenticationResponseGrant.Identity.Name : "NULL"));
            TraceUtility.WriteTrace(this.GetType(), "Invoke(IOwinContext)", null, detail, TraceUtility.TraceType.Watch);

            return Next.Invoke(context);
        }
    }
}
