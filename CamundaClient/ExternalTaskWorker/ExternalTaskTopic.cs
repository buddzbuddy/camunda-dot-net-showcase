namespace Camunda
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class ExternalTaskTopic : System.Attribute
    {
        public string TopicName;

        public ExternalTaskTopic(string topicName)
        {
            this.TopicName = topicName;
        }
    }
}
