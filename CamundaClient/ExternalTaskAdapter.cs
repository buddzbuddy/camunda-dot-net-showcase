namespace Camunda
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class ExternalTaskAdapter : System.Attribute
    {
        public string TopicName;

        public ExternalTaskAdapter(string topicName)
        {
            this.TopicName = topicName;
        }
    }
}
