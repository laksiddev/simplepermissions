using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public class MockAuthenticationOptions : AuthenticationOptions
    {
        private const string __defaultAuthenticationType = "Custom";

        public MockAuthenticationOptions(string userName, string userId)
            : base(__defaultAuthenticationType)
        {
            Description.Caption = __defaultAuthenticationType;
            CallbackPath = new PathString("/api/Login");
            AuthenticationMode = AuthenticationMode.Passive;
            UserName = userName;
            UserId = userId;
        }

        public PathString CallbackPath { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}
