using CamundaClient.Dto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CamundaClient.Requests;

namespace CamundaClient.Service
{

    public class HumanTaskService
    {
        private CamundaClientHelper helper;

        public HumanTaskService(CamundaClientHelper client)
        {
            this.helper = client;
        }

        public IList<HumanTask> LoadTasks()
        {
            HttpClient http = helper.HttpClient("task/");

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var tasks = response.Content.ReadAsAsync<IEnumerable<HumanTask>>().Result;
                http.Dispose();
                return new List<HumanTask>(tasks);
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                http.Dispose();
                return new List<HumanTask>();
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

                Dictionary < string, object>  variables = new Dictionary<string, object>();
                foreach (var variable in variableResponse)
                {
                    if (variable.Value.Type=="object")
                    {
                        string stringValue = (string)variable.Value.Value;
                        // lets assume we only work with JSON serialized values 
                        stringValue = stringValue.Remove(stringValue.Length - 1).Remove(0, 1); // remove one bracket from {{ and }}
                        JToken jsonObject = JContainer.Parse(stringValue);

                        variables.Add(variable.Key, jsonObject);
                    }
                    else { 
                        variables.Add(variable.Key, variable.Value.Value);
                    }
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

        public void Complete(String taskId, Dictionary<string, object> variables)
        {
            HttpClient http = helper.HttpClient("task/" + taskId + "/complete");

            var request = new CompleteRequest();
            request.Variables = CamundaClientHelper.ConvertVariables(variables);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (!response.IsSuccessStatusCode)
            {
                //var errorMsg = response.Content.ReadAsStringAsync();
                http.Dispose();
                throw new EngineException(response.ReasonPhrase);
            }
            http.Dispose();
        }
    }


}
