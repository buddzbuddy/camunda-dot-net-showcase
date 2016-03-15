using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda
{
    class ExternalTaskWorker
    {
        private String workerId = Guid.NewGuid().ToString();
        private String topicName;
        private Adapter adapter;
        private ExternalTaskService service;

        private Timer taskQueryTimer;
        private long pollingIntervalInMilliseconds = 5 * 1000; // every 10 seconds
        private int maxDegreeOfParallelism = 2;
        private int maxTasksToFetchAtOnce = 10;
        private long lockDurationInMilliseconds = 5* 60 * 1000; // 5 minutes

        public ExternalTaskWorker(ExternalTaskService service, String topicName, Adapter adapter)
        {
            this.adapter = adapter;
            this.topicName = topicName;
            this.service = service;
        }

        public void DoPolling()
        {
            // Query External Tasks
            IList<ExternalTask> tasks = service.FetchAndLockTasks(workerId, maxTasksToFetchAtOnce, topicName, lockDurationInMilliseconds, new List<String>());

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
            adapter.Execute(externalTask);
            // TODO: catch exception and handle it

            // report successfull execution
            service.Complete(workerId, externalTask.Id, new List<Variable>());
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
