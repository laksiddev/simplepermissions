using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Open.SPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            TraceUtility.WriteTrace(typeof(ApplicationUserManager), "Create(IdentityFactoryOptions<ApplicationUserManager>, IOwinContext)", TraceUtility.TraceType.Begin);
            var manager = new ApplicationUserManager(new CustomUserStore(context.Get<ApplicationCustomContext>()));

            // Configure the application user manager
            manager.UserValidator = new UserValidator<ApplicationUser, string>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            manager.PasswordValidator = new PasswordValidator
            {
                RequireDigit = false,
                RequiredLength = 4,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, string>(dataProtectionProvider.Create("TEST"));
            }

            TraceUtility.WriteTrace(typeof(ApplicationUserManager), "Create(IdentityFactoryOptions<ApplicationUserManager>, IOwinContext)", TraceUtility.TraceType.End);
            return manager;
        }
    }

    public class ApplicationUser : IUser<string>
    {
        public ApplicationUser() : this(null, null)
        {
        }

        public ApplicationUser(string userName, string id)
        {
            _userName = userName;
            _id = id;
        }

        protected string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        protected string _id;
        public string Id
        {
            get { return _id; }
        }

        internal Task<System.Security.Claims.ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            TraceUtility.WriteTrace(this.GetType(), "GenerateUserIdentityAsync(ApplicationUserManager)", TraceUtility.TraceType.Begin);
            ClaimsIdentityFactory<ApplicationUser, string> factory = new ClaimsIdentityFactory<ApplicationUser, string>();
            ApplicationUser user = new ApplicationUser("BOGUS", "bougus1234");
            TraceUtility.WriteTrace(this.GetType(), "GenerateUserIdentityAsync(ApplicationUserManager)", TraceUtility.TraceType.End);
            return factory.CreateAsync(manager, user, "Custom");
        }
    }
}
