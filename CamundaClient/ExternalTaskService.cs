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

    public class ExternalTaskService
    {
        private CamundaClient client;

        public ExternalTaskService(CamundaClient client)
        {
            this.client = client;
        }

        public IList<ExternalTask> FetchAndLockTasks(String workerId, int maxTasks, String topicName, long lockDurationInMilliseconds, List<String> variablesToFetch)
        {
            HttpClient http = client.HttpClient("external-task/fetchAndLock");

            FetchAndLockRequest request = new FetchAndLockRequest();
            request.workerId = workerId;
            request.maxTasks = maxTasks;
            FetchAndLockTopic topic = new FetchAndLockTopic();
            topic.topicName = topicName;
            topic.lockDuration = lockDurationInMilliseconds;
            topic.variables = variablesToFetch;
            request.topics.Add(topic);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var tasks = response.Content.ReadAsAsync<IEnumerable<ExternalTask>>().Result;
                return new List<ExternalTask>(tasks);
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return new List<ExternalTask>();
            }

        }

        private class FetchAndLockRequest
        {
            public String workerId;
            public int maxTasks;
            public List<FetchAndLockTopic> topics = new List<FetchAndLockTopic>();
        }

  

        private class FetchAndLockTopic
        {
            public String topicName;
            public long lockDuration;
            public List<String> variables;
        }

        internal void Complete(string workerId, string externalTaskid, List<Variable> variablesToPassToProcess)
        {
            HttpClient http = client.HttpClient("external-task/"+externalTaskid+"/complete");

            CompleteRequest request = new CompleteRequest();
            request.workerId = workerId;
            //request.variables = variablesToPassToProcess;

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not complete external Task: " + response.ReasonPhrase);
                //TODO raise exception
            }
        }

        private class CompleteRequest
        {
            public List<Variable> variables { get; set; }
            public string workerId { get; set; }
        }

    }

}
