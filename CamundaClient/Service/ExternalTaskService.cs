using CamundaClient.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

            var lockRequest = new FetchAndLockRequest();
            lockRequest.WorkerId = workerId;
            lockRequest.MaxTasks = maxTasks;
            var lockTopic = new FetchAndLockTopic();
            lockTopic.TopicName = topicName;
            lockTopic.LockDuration = lockDurationInMilliseconds;
            lockTopic.Variables = variablesToFetch;
            lockRequest.Topics.Add(lockTopic);
            try {
                var requestContent = new StringContent(JsonConvert.SerializeObject(lockRequest), Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
                HttpResponseMessage response = http.PostAsync("", requestContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var tasks = JsonConvert.DeserializeObject<IEnumerable<ExternalTask>>(response.Content.ReadAsStringAsync().Result);

                    http.Dispose();
                    return new List<ExternalTask>(tasks);
                }
                else
                {
                    http.Dispose();
                    throw new EngineException("Could not fetch and lock tasks: " + response.ReasonPhrase);
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

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
            HttpResponseMessage response = http.PostAsync("", requestContent).Result;
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

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, CamundaClientHelper.CONTENT_TYPE_JSON);
            HttpResponseMessage response = http.PostAsync("", requestContent).Result;
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new EngineException("Could not report failure for external Task: " + response.ReasonPhrase);
            }
        }
    }
}
