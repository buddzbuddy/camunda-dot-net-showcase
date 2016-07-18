namespace CamundaClient.Worker
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class ExternalTaskTopic : System.Attribute
    {
        public string TopicName;
        public int Retries = 5; // default: 5 times
        public long RetryTimeout = 10 * 1000; // default: 10 seconds

        public ExternalTaskTopic(string topicName)
        {
            this.TopicName = topicName;
        }

        public ExternalTaskTopic(string topicName, int retries, long retryTimeout)
        {
            this.TopicName = topicName;
            this.Retries = retries;
            this.RetryTimeout = retryTimeout;
        }
    }
}
