using System.Collections.Generic;

namespace Camunda
{
    public class HumanTask
    {
        public string id { get; set; }
        public string name { get; set; }
        public string assignee { get; set; }
        public string owner { get; set; }
        public string created { get; set; }
        public string due { get; set; }
        public string followUp { get; set; }
        public string description { get; set; }
        public string priority { get; set; }
        public string formKey { get; set; }

        public override string ToString()
        {
            return "HumanTask [Id=" + id + ", Name=" + name + "]";
        }
    }



}
