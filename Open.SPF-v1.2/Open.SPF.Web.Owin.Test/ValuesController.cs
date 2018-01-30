using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Net.Http;
using Microsoft.Owin;
using Open.SPF.Utility;


namespace Open.SPF.Web.Test.Owin
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            if ((base.Request != null) && (base.Request.GetOwinContext() != null))
            {
                IOwinContext context = Request.GetOwinContext();

                Microsoft.Owin.IOwinRequest request = base.Request.GetOwinContext().Request;
                if (context.Authentication.User != null)
                {
                    Console.WriteLine("HERE");
                }
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }

        public override Task<HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
        {
            IOwinContext owinContext = controllerContext.Request.GetOwinContext();

            string detail = String.Format("context.Request.User.Identity.Name: {0}, context.Authentication.User.Identity.Name: {1}, context.Authentication.AuthenticationResponseGrant.Identity.Name: {2}",
                (((owinContext.Request != null) && (owinContext.Request.User != null)) ? owinContext.Request.User.Identity.Name : "NULL"),
                (((owinContext.Authentication.User != null) && (owinContext.Authentication.User.Identity != null)) ? owinContext.Authentication.User.Identity.Name : "NULL"),
                (((owinContext.Authentication.AuthenticationResponseGrant != null) && (owinContext.Authentication.AuthenticationResponseGrant.Identity != null)) ? owinContext.Authentication.AuthenticationResponseGrant.Identity.Name : "NULL"));
            TraceUtility.WriteTrace(this.GetType(), "ExecuteAsync(HttpControllerContext, CancellationToken)", null, detail, TraceUtility.TraceType.Watch);

            Task<HttpResponseMessage> task = base.ExecuteAsync(controllerContext, cancellationToken);

            return task;
        }

    }
}
