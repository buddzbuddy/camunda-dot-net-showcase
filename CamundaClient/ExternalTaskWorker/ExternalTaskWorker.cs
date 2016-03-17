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

        public ExternalTaskWorker(ExternalTaskService service, ExternalTaskAdapter adapter, String topicName, String[] variablesToFetch)
        {
            this.adapter = adapter;
            this.topicName = topicName;
            this.variablesToFetch = variablesToFetch;
            this.service = service;
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
            Dictionary < string, object>  resultVariables = new Dictionary<string, object>();

            Console.WriteLine("Execute External Task from topic '" + topicName + "': " + externalTask + "..");
            adapter.Execute(externalTask, ref resultVariables);
            Console.WriteLine("..finished External Task " + externalTask.id);

            // TODO: catch exception and handle it


            service.Complete(workerId, externalTask.id, resultVariables);
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
