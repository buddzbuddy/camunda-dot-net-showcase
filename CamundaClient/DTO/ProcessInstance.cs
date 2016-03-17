using System.Collections.Generic;

namespace Camunda
{
    public class ProcessInstance
    {
        public string id { get; set; }
        public string businessKey { get; set; }

        public override string ToString()
        {
            return "ProcessInstance [Id=" + id + ", BusinessKey=" + businessKey + "]";
        }
    }

}
