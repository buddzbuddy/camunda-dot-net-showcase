using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Camunda
{

    public class BpmnWorkflowService
    {
        private CamundaClient client;

        public BpmnWorkflowService(CamundaClient client)
        {
            this.client = client;
        }

        private class StartProcessInstanceRequest
        {
            public Dictionary<string, Variable> variables;
        }


        public string StartProcessInstance(String processDefinitionKey, Dictionary<string, object> variables)
        {
            HttpClient http = client.HttpClient("process-definition/key/" + processDefinitionKey + "/start");

            StartProcessInstanceRequest request = new StartProcessInstanceRequest();
            request.variables = client.convertVariables(variables);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (response.IsSuccessStatusCode)
            {
                var processInstance = response.Content.ReadAsAsync<ProcessInstance>().Result;
                return processInstance.id;
            }
            else
            {
                return null;
            }

        }

        public Dictionary<string, object> LoadVariables(string taskId)
        {
            HttpClient http = client.HttpClient("task/" + taskId + "/variables");

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var variableResponse = response.Content.ReadAsAsync<Dictionary<string, Variable>>().Result;

                Dictionary<string, object> variables = new Dictionary<string, object>();
                foreach (var variable in variableResponse)
                {
                    variables.Add(variable.Key, variable.Value.value);
                }
                return variables;
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }
    }


}
