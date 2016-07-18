using CamundaClient.Service;
using CamundaClient.Worker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamundaClient
{

    public class CamundaEngineClient
    {
        public static string DEFAULT_URL = "http://localhost:8080/engine-rest/engine/default/";
        public static string COCKPIT_URL = "http://localhost:8080/camunda/app/cockpit/default/";

        private IList<ExternalTaskWorker> workers = new List<ExternalTaskWorker>();
        private string RestUrl;
        private string RestUsername;
        private string RestPassword;
        private CamundaClientHelper helper;

        public CamundaEngineClient()
        {
            this.RestUrl = DEFAULT_URL;
            helper = new CamundaClientHelper(this.RestUrl, this.RestUsername, this.RestPassword);
        }

        public CamundaEngineClient(string restUrl, string userName, string password)
        {
            this.RestUrl = restUrl;
            this.RestUsername = userName;
            this.RestPassword = password;
            helper = new CamundaClientHelper(this.RestUrl, this.RestUsername, this.RestPassword);
        }

        public BpmnWorkflowService BpmnWorkflowService()
        {
            return new BpmnWorkflowService(helper);
        }

        public HumanTaskService HumanTaskService()
        {
            return new HumanTaskService(helper);
        }

        public RepositoryService RepositoryService()
        {
            return new RepositoryService(helper);
        }

        public ExternalTaskService ExternalTaskService()
        {
            return new ExternalTaskService(helper);
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
                var retries = taskWorker.Attributes.FirstOrDefault().Retries;
                var retryTimeout = taskWorker.Attributes.FirstOrDefault().RetryTimeout;

                string[] variablesToFetch = null;
                var variableRequirements = taskWorker.Type.GetCustomAttributes(typeof(ExternalTaskVariableRequirements), true)
                    .FirstOrDefault() as ExternalTaskVariableRequirements;
                if (variableRequirements != null)
                {
                    variablesToFetch = variableRequirements.VariablesToFetch;
                }

                var constructor = taskWorker.Type.GetConstructor(Type.EmptyTypes);
                IExternalTaskAdapter adapter = (IExternalTaskAdapter)constructor.Invoke(null);

                // Now register it!
                Console.WriteLine("Register Task Worker for Topic '" + workerTopicName + "'");
                ExternalTaskWorker worker = new ExternalTaskWorker(ExternalTaskService(), adapter, workerTopicName, retries, retryTimeout, variablesToFetch);
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

    }
}