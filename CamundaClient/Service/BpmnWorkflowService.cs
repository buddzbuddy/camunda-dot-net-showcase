using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CamundaClient.Requests;

namespace CamundaClient.Service
{

    public class BpmnWorkflowService
    {
        private CamundaClientHelper helper;

        public BpmnWorkflowService(CamundaClientHelper client)
        {
            this.helper = client;
        }

        public string StartProcessInstance(String processDefinitionKey, Dictionary<string, object> variables)
        {
            HttpClient http = helper.HttpClient("process-definition/key/" + processDefinitionKey + "/start");

            var request = new CompleteRequest();
            request.Variables = CamundaClientHelper.ConvertVariables(variables);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (response.IsSuccessStatusCode)
            {
                var processInstance = response.Content.ReadAsAsync<ProcessInstance>().Result;
                http.Dispose();
                return processInstance.Id;
            }
            else
            {
                //var errorMsg = response.Content.ReadAsStringAsync();
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
                    variables.Add(variable.Key, variable.Value.Value);
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
