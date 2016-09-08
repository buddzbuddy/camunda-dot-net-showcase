using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CamundaClient.Service
{

    public class ExternalTaskService
    {
        private CamundaClientHelper helper;

        public ExternalTaskService(CamundaClientHelper client)
        {
            this.helper = client;
        }

        public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName, long lockDurationInMilliseconds, List<string> variablesToFetch)
        {
            HttpClient http = helper.HttpClient("external-task/fetchAndLock");

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
                    http.Dispose();
                    return new List<ExternalTask>(tasks);
                }
                else
                {
                    return new List<ExternalTask>();
                }
            }
            catch (Exception ex)
            {
                http.Dispose();
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

        public void Complete(string workerId, string externalTaskId, Dictionary<string, object> variablesToPassToProcess)
        {
            HttpClient http = helper.HttpClient("external-task/" + externalTaskId + "/complete");

            CompleteRequest request = new CompleteRequest();
            request.workerId = workerId;
            request.variables = helper.convertVariables(variablesToPassToProcess);

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new EngineException("Could not complete external Task: " + response.ReasonPhrase);
            }
        }

        public void Failure(string workerId, string externalTaskId, string errorMessage, int retries, long retryTimeout)
        {
            HttpClient http = helper.HttpClient("external-task/" + externalTaskId + "/failure");

            FailureRequest request = new FailureRequest();
            request.workerId = workerId;
            request.errorMessage = errorMessage;
            request.retries = retries;
            request.retryTimeout = retryTimeout;

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new EngineException("Could not report failure for external Task: " + response.ReasonPhrase);
            }
        }

        private class CompleteRequest
        {
            public Dictionary<string, Variable> variables { get; set; }
            public string workerId { get; set; }
        }
        private class FailureRequest
        {
            public string workerId { get; set; }
            public string errorMessage { get; set; }
            public int retries { get; set; }
            public long retryTimeout{ get; set; }
        }
    }

}
