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
    public class ApplicationCustomContext : Microsoft.Owin.IOwinContext, IDisposable
    {
        private static ApplicationCustomContext _instance;

        public ApplicationCustomContext()
        {
            TraceUtility.WriteTrace(this.GetType(), "new ApplicationCustomContext()", TraceUtility.TraceType.Begin);
            _environment = new Dictionary<string, object>();
            TraceUtility.WriteTrace(this.GetType(), "new ApplicationCustomContext()", TraceUtility.TraceType.End);
        }

        public Microsoft.Owin.Security.IAuthenticationManager Authentication
        {
            get { throw new NotImplementedException(); }
        }

        Dictionary<string, object> _environment;
        public IDictionary<string, object> Environment
        {
            get { return _environment; }
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Owin.IOwinRequest Request
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Owin.IOwinResponse Response
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Owin.IOwinContext Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        private System.IO.TextWriter _traceOutput = null;
        public System.IO.TextWriter TraceOutput
        {
            get
            {
                return _traceOutput;
            }
            set
            {
                _traceOutput = value;
            }
        }

        internal static ApplicationCustomContext Create()
        {
            TraceUtility.WriteTrace(typeof(ApplicationCustomContext), "Create()", TraceUtility.TraceType.Begin);
            if (_instance == null)
                _instance = new ApplicationCustomContext();

            TraceUtility.WriteTrace(typeof(ApplicationCustomContext), "Create()", TraceUtility.TraceType.End);
            return _instance;
        }

        public void Dispose()
        {
            TraceUtility.WriteTrace(this.GetType(), "Dispose()", TraceUtility.TraceType.Watch);
            // nothing to dispose
        }
    }
}
