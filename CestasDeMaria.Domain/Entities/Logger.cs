using System.Diagnostics;
using System.Reflection;

namespace CestasDeMaria.Domain.Entities
{
    public class Logger : BaseEntity
    {
        public Logger()
        {
        }

        public Logger(Exception ex)
            : this()
        {
            Message = ex.Message;
            Stacktrace = ex.StackTrace.ToString();
            Methodname = ex.TargetSite?.Name;
            Classname = ex.TargetSite?.DeclaringType?.FullName;
            Created = DateTime.Now;
            Updated = DateTime.Now;
        }

        public Logger(string message, long personCode)
            : this()
        {
            Message = message;
            Adminid = personCode;
        }

        public Logger(string message, string methodName = "", string className = "", int frame = 1)
            : this()
        {
            Message = message;

            if (methodName == null)
            {
                StackTrace stackTrace = new StackTrace();

                Stacktrace = stackTrace.ToString();

                StackFrame frameStack = stackTrace.GetFrame(frame); // Get the calling method frame

                MethodBase method = frameStack?.GetMethod();

                Methodname = method?.Name;
                Classname = method?.DeclaringType?.Name;
            }
            else
            {
                Methodname = methodName;
                Classname = className;
            }
        }
        public string Message { get; set; }
        public long Adminid { get; set; }
        public string Classname { get; set; }
        public string Methodname { get; set; }
        public string Methodsignature { get; set; }
        public string Methodparameters { get; set; }
        public string Stacktrace { get; set; }
    }
}
