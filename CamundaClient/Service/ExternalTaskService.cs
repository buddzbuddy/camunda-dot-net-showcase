using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Camunda
{

    public class ExternalTaskService
    {
        private CamundaClient client;

        public ExternalTaskService(CamundaClient client)
        {
            this.client = client;
        }

        public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName, long lockDurationInMilliseconds, List<string> variablesToFetch)
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
            try {
                HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var tasks = response.Content.ReadAsAsync<IEnumerable<ExternalTask>>().Result;
                    return new List<ExternalTask>(tasks);
                }
                else
                {
                    return new List<ExternalTask>();
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle Exception, add backoff
                return new List<ExternalTask>();
            }
        }

        private class FetchAndLockRequest
        {
            public string workerId;
            public int maxTasks;
            public List<FetchAndLockTopic> topics = new List<FetchAndLockTopic>();
        }

        private class FetchAndLockTopic
        {
            public string topicName;
            public long lockDuration;
            public List<string> variables;
        }

        internal void Complete(string workerId, string externalTaskId, Dictionary<string, object> variablesToPassToProcess)
        {
            HttpClient http = client.HttpClient("external-task/" + externalTaskId + "/complete");

            CompleteRequest request = new CompleteRequest();
            request.workerId = workerId;
            request.variables = client.convertVariables(variablesToPassToProcess);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not complete external Task: " + response.ReasonPhrase);
                //TODO raise exception
            }
        }

        private class CompleteRequest
        {
            public Dictionary<string, Variable> variables { get; set; }
            public string workerId { get; set; }
        }

    }

}
