using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Open.SPF.Web.Test.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.Use(typeof(Open.SPF.Web.Test.Owin.TestMiddleware));
            ConfigureAuth(appBuilder);

            appBuilder.UseWebApi(config);
        }

         public void ConfigureAuth(IAppBuilder app) 
         { 
             // Configure the db context and user manager to use a single instance per request 
             app.CreatePerOwinContext(ApplicationCustomContext.Create); 
             app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);


             app.UseMockAuthentication(new MockAuthenticationOptions("John Doe", "jdoe42")); 

             // Enable the application to use a cookie to store information for the signed in user 
             // and to use a cookie to temporarily store information about a user logging in with a third party login provider 
             // Configure the sign in cookie 
             app.UseCookieAuthentication(new CookieAuthenticationOptions
             {
                 AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                 LoginPath = new PathString("/api/Login"),
                 Provider = new CookieAuthenticationProvider
                 {
                     OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                         validateInterval: TimeSpan.FromMinutes(30),
                         regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                 }
             });

             app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie); 
 
 
             // Uncomment the following lines to enable logging in with third party login providers 
             //app.UseMicrosoftAccountAuthentication( 
             //    clientId: "", 
             //    clientSecret: ""); 
 
             //app.UseTwitterAuthentication( 
             //   consumerKey: "", 
             //   consumerSecret: ""); 
 

             //app.UseFacebookAuthentication( 
             //   appId: "", 
             //   appSecret: ""); 
 
 
             //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions() 
             //{ 
             //    ClientId = "", 
             //    ClientSecret = "" 
             //}); 
 
         } 
 
  }
}
