using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CE.Dto
{
    public abstract class BaseDto
    {
        [Key]
        public abstract int ID { get; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; private set; }

        public void AppendMessage(string message)
        {
            if (this.Message == null || (this.Message != null && !this.Message.Contains(message)))
            {
                if (!string.IsNullOrWhiteSpace(this.Message) && !this.Message.Trim().EndsWith("<br>") && !this.Message.Trim().EndsWith("<br/>") && !this.Message.Trim().EndsWith("<br />"))
                {
                    this.Message += "<br>";
                }
                this.Message += message;
            }
        }

        public string ToSearchString()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //BindingFlags flags = BindingFlags.Public;
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    var value = property.GetValue(this);
                    if (value != null)
                    {
                        sb.Append(value.ToString().ToLower() + " ");
                    }
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return base.ToString();
            }
        }
    }
}
