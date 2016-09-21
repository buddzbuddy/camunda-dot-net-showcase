using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CamundaClient.Requests;

namespace CamundaClient.Service
{

    public class ExternalTaskService
    {
        private CamundaClientHelper helper;

        public ExternalTaskService(CamundaClientHelper client)
        {
            this.helper = client;
        }

        public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName, long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch)
        {
            HttpClient http = helper.HttpClient("external-task/fetchAndLock");

            FetchAndLockRequest request = new FetchAndLockRequest();
            request.WorkerId = workerId;
            request.MaxTasks = maxTasks;
            FetchAndLockTopic topic = new FetchAndLockTopic();
            topic.TopicName = topicName;
            topic.LockDuration = lockDurationInMilliseconds;
            topic.Variables = variablesToFetch;
            request.Topics.Add(topic);
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
                Console.WriteLine(ex.Message);
                // TODO: Handle Exception, add back off
                return new List<ExternalTask>();
            }
        }

        public void Complete(string workerId, string externalTaskId, Dictionary<string, object> variablesToPassToProcess)
        {
            HttpClient http = helper.HttpClient("external-task/" + externalTaskId + "/complete");

            CompleteRequest request = new CompleteRequest();
            request.WorkerId = workerId;
            request.Variables = CamundaClientHelper.ConvertVariables(variablesToPassToProcess);

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
            request.WorkerId = workerId;
            request.ErrorMessage = errorMessage;
            request.Retries = retries;
            request.RetryTimeout = retryTimeout;

            HttpResponseMessage response = http.PostAsJsonAsync("", request).Result;
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new EngineException("Could not report failure for external Task: " + response.ReasonPhrase);
            }
        }
    }
}
