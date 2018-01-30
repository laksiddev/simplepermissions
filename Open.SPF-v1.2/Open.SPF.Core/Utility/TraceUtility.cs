using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Utility
{
    public class TraceUtility
    {
        public static void WriteTrace(Type sender, string methodName, TraceType traceType)
        {
            WriteTrace(sender, methodName, null, null, traceType);
        }

        public static void WriteTrace(Type sender, string methodName, string wrappingMethod, string detail, TraceType traceType)
        {
            StringBuilder sbMessage = new StringBuilder();
            if (sender != null)
            {
                sbMessage.Append(sender.FullName);
            }
            else 
            { 
                sbMessage.Append("UnknownClass"); 
            }
            sbMessage.Append(".");

            if (!String.IsNullOrEmpty(methodName))
            {
                sbMessage.Append(methodName);
                if (!methodName.EndsWith(")"))
                {
                    sbMessage.Append("()");
                }
            }
            else
            {
                sbMessage.Append("UnknownMethod()");
            }

            if (!String.IsNullOrEmpty(wrappingMethod))
            {
                sbMessage.Append(" wrapping ");
                sbMessage.Append(wrappingMethod);
                if (!wrappingMethod.EndsWith(")"))
                {
                    sbMessage.Append("()");
                }
            }

            sbMessage.Append(".");

            if (!String.IsNullOrEmpty(detail))
            {
                sbMessage.Append(" ");
                sbMessage.Append(detail);
            }

            System.Diagnostics.Trace.WriteLine(sbMessage.ToString(), "APPTRACE - " + traceType.ToString());
        }

        public enum TraceType
        {
            Begin,
            End,
            Watch,
            Warning
        }
    }
}
