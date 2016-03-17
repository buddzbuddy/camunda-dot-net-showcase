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

    public class HumanTaskService
    {
        private CamundaClient client;

        public HumanTaskService(CamundaClient client)
        {
            this.client = client;
        }

        public IList<HumanTask> LoadTasks()
        {
            HttpClient http = client.HttpClient("task/");

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var tasks = response.Content.ReadAsAsync<IEnumerable<HumanTask>>().Result;
                return new List<HumanTask>(tasks);
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return new List<HumanTask>();
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

                Dictionary < string, object>  variables = new Dictionary<string, object>();
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

        private class CompleteTaskRequest
        {
            public Dictionary<string, Variable> variables;
        }


        public void Complete(String taskId, Dictionary<string, object> variables)
        {
            HttpClient http = client.HttpClient("/task/" + taskId + "/complete");

            CompleteTaskRequest request = new CompleteTaskRequest();
            request.variables = client.convertVariables(variables);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (!response.IsSuccessStatusCode)
            {
                // ?!
            }

        }
    }


}
