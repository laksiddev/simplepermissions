using Microsoft.AspNet.Identity;
using Open.SPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Web.Test.Owin
{
    public class CustomUserStore : IUserStore<ApplicationUser, string>
    {
        public CustomUserStore(ApplicationCustomContext context)
        {
            TraceUtility.WriteTrace(this.GetType(), "new CustomUserStore(ApplicationCustomContext)", TraceUtility.TraceType.Watch);
        }

        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            TraceUtility.WriteTrace(this.GetType(), "Dispose()", TraceUtility.TraceType.Watch);
            // Nothing to dispose
        }
    }
}
