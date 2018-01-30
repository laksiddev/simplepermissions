using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace Open.SPF.Web.Test.Owin
{
    public class LoginController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            IOwinContext context = Request.GetOwinContext();

            Microsoft.Owin.IOwinRequest request = base.Request.GetOwinContext().Request;
            if (context.Authentication.User != null)
            {
                System.Diagnostics.Debug.WriteLine("HERE");
            }

            return new string[0];
        }
    }
}
