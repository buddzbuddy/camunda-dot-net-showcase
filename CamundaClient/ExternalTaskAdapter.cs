using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //public object TopicName { get; internal set; }
    }
}
