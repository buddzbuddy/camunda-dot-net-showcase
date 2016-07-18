using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CamundaClient.Service
{

    public class BpmnWorkflowService
    {
        private CamundaClientHelper helper;

        public BpmnWorkflowService(CamundaClientHelper client)
        {
            this.helper = client;
        }

        private class StartProcessInstanceRequest
        {
            public Dictionary<string, Variable> variables;
        }


        public string StartProcessInstance(String processDefinitionKey, Dictionary<string, object> variables)
        {
            HttpClient http = helper.HttpClient("process-definition/key/" + processDefinitionKey + "/start");

            StartProcessInstanceRequest request = new StartProcessInstanceRequest();
            request.variables = helper.convertVariables(variables);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (response.IsSuccessStatusCode)
            {
                var processInstance = response.Content.ReadAsAsync<ProcessInstance>().Result;
                http.Dispose();
                return processInstance.id;
            }
            else
            {
                var errorMsg = response.Content.ReadAsStringAsync();
                http.Dispose();
                throw new EngineException(response.ReasonPhrase);
            }

        }

        public Dictionary<string, object> LoadVariables(string taskId)
        {
            HttpClient http = helper.HttpClient("task/" + taskId + "/variables");

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
                http.Dispose();
                return variables;
            }
            else
            {
                http.Dispose();
                return new Dictionary<string, object>();
            }
        }
    }


}
