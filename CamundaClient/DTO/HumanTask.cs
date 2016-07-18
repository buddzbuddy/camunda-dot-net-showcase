using System;
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class HumanTask
    {
        public string id { get; set; }
        public string name { get; set; }
        public string assignee { get; set; }
        public string owner { get; set; }
        public DateTime? created { get; set; }
        public DateTime? due { get; set; }
        public DateTime? followUp { get; set; }
        public string description { get; set; }
        public string priority { get; set; }
        public string formKey { get; set; }
        public string processInstanceId { get; set; }
        public string processDefinitionId { get; set; }
        public string taskDefinitionKey { get; set; }
        // more attributes see https://docs.camunda.org/manual/latest/reference/rest/task/get-query/

        public override string ToString()
        {
            return "HumanTask [Id=" + id + ", Name=" + name + "]";
        }
    }



}
