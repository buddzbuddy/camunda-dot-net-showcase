using CamundaClient.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
            public string businessKey;
        }

        public string StartProcessInstance(String processDefinitionKey, Dictionary<string, object> variables)
        {
            return StartProcessInstance(processDefinitionKey, null, variables);
        }

        public string StartProcessInstance(String processDefinitionKey, String businessKey, Dictionary<string, object> variables)
        {
            HttpClient http = helper.HttpClient("process-definition/key/" + processDefinitionKey + "/start");

            StartProcessInstanceRequest request = new StartProcessInstanceRequest();
            request.businessKey = businessKey;
            request.variables = helper.convertVariables(variables);

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
            HttpResponseMessage response = http.PostAsync("", requestContent).Result;
            if (response.IsSuccessStatusCode)
            {
                var processInstance = JsonConvert.DeserializeObject<ProcessInstance>(
                    response.Content.ReadAsStringAsync().Result);

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
                var variableResponse = JsonConvert.DeserializeObject< Dictionary<string, Variable>>(
                    response.Content.ReadAsStringAsync().Result);

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
                throw new EngineException("Could not load variable: " + response.ReasonPhrase);
            }
        }
    }


}
