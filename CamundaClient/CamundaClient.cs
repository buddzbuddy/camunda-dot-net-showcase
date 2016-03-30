using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Camunda
{

    public class CamundaClient
    {
        public string DEFAULT_URL = "http://localhost:8080/engine-rest/engine/default/";

        private IList<ExternalTaskWorker> workers = new List<ExternalTaskWorker>();
        public string RestUrl { get; }
        public string RestUsername { get; }
        public string RestPassword { get; }

        public CamundaClient()
        {
            this.RestUrl = DEFAULT_URL;
        }
        public CamundaClient(string restUrl, string username, string password)
        {
            this.RestUrl = restUrl;
            this.RestUsername = username;
            this.RestPassword = password;
        }

        public BpmnWorkflowService BpmnWorkflowService()
        {
            return new BpmnWorkflowService(this);
        }

        public HumanTaskService HumanTaskService()
        {
            return new HumanTaskService(this);
        }

        public RepositoryService RepositoryService()
        {
            return new RepositoryService(this);
        }

        public ExternalTaskService ExternalTaskService()
        {
            return new ExternalTaskService(this);
        }

        public void Startup()
        {
            this.StartWorkers();
            this.RepositoryService().AutoDeploy();
        }

        public void Shutdown()
        {
            this.StopWorkers();
        }

        public void StartWorkers()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            // find all classes with CustomAttribute [ExternalTask("name")]
            var externalTaskWorkers =
                // from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(ExternalTaskTopic), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<ExternalTaskTopic>() };

            foreach (var taskWorker in externalTaskWorkers)
            {
                var workerTopicName = taskWorker.Attributes.FirstOrDefault().TopicName;

                string[] variablesToFetch = null;
                var variableRequirements = taskWorker.Type.GetCustomAttributes(typeof(ExternalTaskVariableRequirements), true)
                    .FirstOrDefault() as ExternalTaskVariableRequirements;
                if (variableRequirements != null)
                {
                    variablesToFetch = variableRequirements.VariablesToFetch;
                }

                var constructor = taskWorker.Type.GetConstructor(Type.EmptyTypes);
                ExternalTaskAdapter adapter = (ExternalTaskAdapter)constructor.Invoke(null);

                // Now register it!
                Console.WriteLine("Register Task Worker for Topic '" + workerTopicName + "'");
                ExternalTaskWorker worker = new ExternalTaskWorker(ExternalTaskService(), adapter, workerTopicName, variablesToFetch);
                workers.Add(worker);
                worker.StartWork();
            }
        }

        public void StopWorkers()
        {
            foreach (ExternalTaskWorker worker in workers)
            {
                worker.StopWork();
            }
        }

        // HELPER METHODS

        public HttpClient HttpClient(string path)
        {
            HttpClient client = null;
            if (RestUsername != null)
            {
                var credentials = new NetworkCredential(RestUsername, RestPassword);
                client = new HttpClient(new HttpClientHandler() { Credentials = credentials });
            }
            else
            {
                client = new HttpClient();
            }
            client.BaseAddress = new Uri(RestUrl + path);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public Dictionary<string, Variable> convertVariables(Dictionary<string, object> variables)
        {
            // report successfull execution
            Dictionary<string, Variable> result = new Dictionary<string, Variable>();
            foreach (var variable in variables)
            {
                Variable camundaVariable = new Variable();
                camundaVariable.value = variable.Value;
                result.Add(variable.Key, camundaVariable);
            }
            return result;
        }
    }
}