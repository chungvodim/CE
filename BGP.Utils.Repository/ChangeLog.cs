using EntityFramework.Audit;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BGP.Utils.Repository
{
    public class ChangeLog
    {
        public ChangeLog()
        {
            Keys = new List<ChangeLogKey>();
            Properties = new List<ChangeLogProperty>();
        }

        [JsonIgnore]
        public AuditAction AuditAction { get; set; }

        public string Action
        {
            get
            {
                if (AuditAction == AuditAction.Added)
                    return "Added";

                if (AuditAction == AuditAction.Deleted)
                    return "Deleted";

                return "Modified";
            }
        }

        public List<ChangeLogKey> Keys { get; set; }
        public List<ChangeLogProperty> Properties { get; set; }
    }

    public class ChangeLogKey
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ChangeLogProperty
    {
        public string Name { get; set; }
        public object Old { get; set; }
        public object New { get; set; }
    }
}