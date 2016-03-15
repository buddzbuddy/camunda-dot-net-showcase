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

    public class CamundaClient
    {
        public string URL = "http://localhost:8080/engine-rest/engine/default/";

        private IList<ExternalTaskWorker> workers = new List<ExternalTaskWorker>();

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

        public void init()
        {
            this.StartWorkers();
        }

        public void StartWorkers()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            // find all classes with CustomAttribute [ExternalTask("name")]
            var externalTaskWorkers =
                // from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(ExternalTaskAdapter), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<ExternalTaskAdapter>() };

            foreach (var taskWorker in externalTaskWorkers)
            {
                var workerTopicName = taskWorker.Attributes.First().TopicName;
                Console.WriteLine("Register Task Worker for Topic '" + workerTopicName + "'");

                var constructor = taskWorker.Type.GetConstructor(Type.EmptyTypes);
                Adapter adapter = (Adapter)constructor.Invoke(null);

                // Now register it!
                ExternalTaskWorker worker = new ExternalTaskWorker(ExternalTaskService(), workerTopicName, adapter);
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

        public HttpClient HttpClient(string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL + path);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }


    }
}