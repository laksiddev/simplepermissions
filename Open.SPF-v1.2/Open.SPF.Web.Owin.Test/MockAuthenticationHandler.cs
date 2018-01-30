using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Open.SPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            TraceUtility.WriteTrace(this.GetType(), "AuthenticateCoreAsync()", TraceUtility.TraceType.Begin);
            // ASP.Net Identity requires the NameIdentitifer field to be set or it won't   
            // accept the external login (AuthenticationManagerExtensions.GetExternalLoginInfo) 
            var identity = new ClaimsIdentity(Options.SignInAsAuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Options.UserId, null, Options.AuthenticationType));
            identity.AddClaim(new Claim(ClaimTypes.Name, Options.UserName));

            AuthenticationProperties properties = Options.StateDataFormat.Unprotect(Request.Query["state"]);
            //properties.RedirectUri = Request.Query["ReturnUrl"];

            TraceUtility.WriteTrace(this.GetType(), "AuthenticateCoreAsync()", TraceUtility.TraceType.End);
            return Task.FromResult(new AuthenticationTicket(identity, properties));
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            TraceUtility.WriteTrace(this.GetType(), "ApplyResponseChallengeAsync()", TraceUtility.TraceType.Begin);
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                // Only react to 401 if there is an authentication challenge for the authentication  
                // type of this handler. 
                if (challenge != null)
                {
                    var state = challenge.Properties;

                    if (string.IsNullOrEmpty(state.RedirectUri))
                    {
                        state.RedirectUri = Request.Uri.ToString();
                    }

                    var stateString = Options.StateDataFormat.Protect(state);

                    Response.Redirect(WebUtilities.AddQueryString(Options.CallbackPath.Value, "state", stateString));
                }
            }


            TraceUtility.WriteTrace(this.GetType(), "ApplyResponseChallengeAsync()", TraceUtility.TraceType.End);
            return Task.FromResult<object>(null);
        }


        public override async Task<bool> InvokeAsync()
        {
            TraceUtility.WriteTrace(this.GetType(), "InvokeAsync()", TraceUtility.TraceType.Begin);
            // This is always invoked on each request. For passive middleware, only do anything if this is 
            // for our callback path when the user is redirected back from the authentication provider. 
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                var ticket = await AuthenticateAsync();

                if (ticket != null)
                {
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

                    if (!String.IsNullOrEmpty(ticket.Properties.RedirectUri))
                    {
                        Response.Redirect(ticket.Properties.RedirectUri);
                    }
                    else if (!String.IsNullOrEmpty(Request.Query["ReturnUrl"]))
                    {
                        Response.Redirect(Request.Query["ReturnUrl"]);
                    }

                    TraceUtility.WriteTrace(this.GetType(), "InvokeAsync()", TraceUtility.TraceType.End);
                    // Prevent further processing by the owin pipeline. 
                    return true;
                }
            }
            TraceUtility.WriteTrace(this.GetType(), "InvokeAsync()", TraceUtility.TraceType.End);
            // Let the rest of the pipeline run. 
            return false;
        }
    }
}
