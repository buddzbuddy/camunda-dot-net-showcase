using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class Deployment
    {
        public string id { get; set; }
        public string name { get; set; }
        public string source { get; set; }

        public override string ToString()
        {
            return "Deployment [Id=" + id + ", Name=" + name + "]";
        }
    }

}
