using System.Collections.Generic;

namespace Camunda
{
    public class ExternalTask
    {
        public string activityId { get; set; }
        public string activityInstanceId { get; set; }
        public string id { get; set; }
        public Dictionary<string, Variable> variables { get; set; }

        public override string ToString()
        {
            return "ExternalTask [Id=" + id + ", ActivityId=" + activityId+"]";
        }
    }

    public class Variable
    {
        // lower case to generate JSON we need
        public string type { get; set; }
        public object value { get; set; }
        public object valueInfo { get; set; }
    }





}
