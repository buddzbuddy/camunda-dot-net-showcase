using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda
{
    class ExternalTaskWorker
    {
        private String workerId = Guid.NewGuid().ToString(); // TODO: Make configurable
        private String topicName;
        private String[] variablesToFetch;
        private ExternalTaskAdapter adapter;
        private ExternalTaskService service;

        private Timer taskQueryTimer;
        private long pollingIntervalInMilliseconds = 1 * 1000; // every second
        private int maxDegreeOfParallelism = 2;
        private int maxTasksToFetchAtOnce = 10;
        private long lockDurationInMilliseconds = 1 * 60 * 1000; // 1 minute
        private ExternalTaskService externalTaskService;
        private string workerTopicName;
        private int retries;
        private long retryTimeout;

        public ExternalTaskWorker(ExternalTaskService service, ExternalTaskAdapter adapter, String topicName, int retries, long retryTimeout, String[] variablesToFetch)
        {
            this.adapter = adapter;
            this.topicName = topicName;
            this.variablesToFetch = variablesToFetch;
            this.service = service;
            this.retries = retries;
            this.retryTimeout = retryTimeout;
        }

        public void DoPolling()
        {
            // Query External Tasks
            IList<ExternalTask> tasks = service.FetchAndLockTasks(workerId, maxTasksToFetchAtOnce, topicName, lockDurationInMilliseconds, new List<string>(variablesToFetch));

            // run them in parallel with a max degree of parallelism
            Parallel.ForEach(
                tasks,
                new ParallelOptions { MaxDegreeOfParallelism = this.maxDegreeOfParallelism },
                externalTask => { Execute(externalTask); }
            );

            // schedule next run
            taskQueryTimer.Change(pollingIntervalInMilliseconds, Timeout.Infinite);
        }

        private void Execute(ExternalTask externalTask)
        {
            Dictionary<string, object> resultVariables = new Dictionary<string, object>();

            Console.WriteLine("Execute External Task from topic '" + topicName + "': " + externalTask + "...");
            try
            {
                adapter.Execute(externalTask, ref resultVariables);
                Console.WriteLine("...finished External Task " + externalTask.id);
                service.Complete(workerId, externalTask.id, resultVariables);
            }
            catch (Exception ex)
            {
                Console.WriteLine("...failed External Task  " + externalTask.id);
                var retriesLeft = retries; // start with default
                if (externalTask.retries.HasValue) // or decrement if retries are already set
                {
                    retriesLeft = externalTask.retries.Value - 1;
                }
                service.Failure(workerId, externalTask.id, ex.Message, retriesLeft, retryTimeout);
            }
        }

        public void StartWork()
        {
            this.taskQueryTimer = new Timer(_ => DoPolling(), null, pollingIntervalInMilliseconds, Timeout.Infinite);
        }

        public void StopWork()
        {
            this.taskQueryTimer.Dispose();
        }
    }
}
