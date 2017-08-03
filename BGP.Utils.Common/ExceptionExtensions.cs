using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class ExceptionExtensions
    {
        public static Exception GetInnermostException(this Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e;
        }
    }

    [Serializable]
    public class ApiException : Exception
    {
        public HttpStatusCode HttpStatus { get; set; }

        public string Error { get; set; }

        public ApiException(HttpStatusCode httpStatus, string error)
        {
            Error = error;
            HttpStatus = httpStatus;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HttpStatus + " " + Message);
            sb.AppendLine(base.ToString());
            return sb.ToString();
        }

    }
}
