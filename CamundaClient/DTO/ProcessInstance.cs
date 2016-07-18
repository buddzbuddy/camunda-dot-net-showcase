using System.Collections.Generic;

namespace CamundaClient.Dto
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
