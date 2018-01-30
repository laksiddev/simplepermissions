using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Open.SPF.Utility;

namespace Open.SPF.Web.Test.Owin
{
    public class MockAuthenticationMiddleware : AuthenticationMiddleware<MockAuthenticationOptions>
    {
        AuthenticationHandler<MockAuthenticationOptions> _handlerInstance;

         public MockAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, MockAuthenticationOptions options) 
             : base(next, options) 
         {
             TraceUtility.WriteTrace(this.GetType(), "new MockAuthenticationMiddleware(OwinMiddleware, IAppBuilder, MockAuthenticationOptions)", TraceUtility.TraceType.Begin);
             if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType)) 
             { 
                 options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(); 
             } 
             if(options.StateDataFormat == null) 
             { 
                 var dataProtector = app.CreateDataProtector(typeof(MockAuthenticationMiddleware).FullName, 
                     options.AuthenticationType); 
 
                 options.StateDataFormat = new PropertiesDataFormat(dataProtector); 
             }
             TraceUtility.WriteTrace(this.GetType(), "new MockAuthenticationMiddleware(OwinMiddleware, IAppBuilder, MockAuthenticationOptions)", TraceUtility.TraceType.End);
         } 
  
         // Called for each request, to create a handler for each request. 
         protected override AuthenticationHandler<MockAuthenticationOptions> CreateHandler() 
         {
             TraceUtility.WriteTrace(this.GetType(), "CreateHandler()", TraceUtility.TraceType.Begin);
             if (_handlerInstance == null)
                _handlerInstance = new MockAuthenticationHandler();

             TraceUtility.WriteTrace(this.GetType(), "CreateHandler()", TraceUtility.TraceType.End);
             return _handlerInstance;
         }
    }
}
