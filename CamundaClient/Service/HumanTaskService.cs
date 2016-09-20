using CamundaClient.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

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
            return LoadTasks(new Dictionary<string, string>());
        }

        public IList<HumanTask> LoadTasks(IDictionary<string, string> queryParameters)
        {
            string s = string.Join("&", queryParameters.Select(x => x.Key + "=" + x.Value));
            HttpClient http = helper.HttpClient("task/?" + s);

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var tasks = JsonConvert.DeserializeObject<IEnumerable < HumanTask >> (
                    response.Content.ReadAsStringAsync().Result);
                http.Dispose();
                return new List<HumanTask>(tasks);
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                http.Dispose();
                throw new EngineException("Could not fetch and lock tasks: " + response.ReasonPhrase);
            }

        }

        public Dictionary<string, object> LoadVariables(string taskId)
        {
            HttpClient http = helper.HttpClient("task/" + taskId + "/variables");

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var variableResponse = JsonConvert.DeserializeObject<Dictionary < string, Variable>> (
                    response.Content.ReadAsStringAsync().Result);

                Dictionary< string, object>  variables = new Dictionary<string, object>();
                foreach (var variable in variableResponse)
                {
                    if (variable.Value.type=="object")
                    {
                        string stringValue = (string)variable.Value.value;
                        // lets assume we only work with JSON serialized values 
                        stringValue = stringValue.Remove(stringValue.Length - 1).Remove(0, 1); // remove one bracket from {{ and }}
                        JToken jsonObject = JContainer.Parse(stringValue);

                        variables.Add(variable.Key, jsonObject);
                    }
                    else { 
                        variables.Add(variable.Key, variable.Value.value);
                    }
                }
                http.Dispose();
                return variables;
            }
            else
            {
                http.Dispose();
                throw new EngineException("Could not fetch and lock tasks: " + response.ReasonPhrase);
            }
        }

        private class CompleteTaskRequest
        {
            public Dictionary<string, Variable> variables;
        }


        public void Complete(String taskId, Dictionary<string, object> variables)
        {
            HttpClient http = helper.HttpClient("task/" + taskId + "/complete");

            CompleteTaskRequest request = new CompleteTaskRequest();
            request.variables = helper.convertVariables(variables);

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
            HttpResponseMessage response = http.PostAsync("", requestContent).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = response.Content.ReadAsStringAsync();
                http.Dispose();
                throw new EngineException(response.ReasonPhrase);
            }
            http.Dispose();
        }
    }


}
