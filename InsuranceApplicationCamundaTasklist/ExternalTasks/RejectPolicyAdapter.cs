using System;
using System.Collections.Generic;
using Camunda;

namespace InsuranceApplicationWpfTasklist
{
    [ExternalTaskTopic("rejectPolicy")]
    [ExternalTaskVariableRequirements("name", "carType", "carManufacturer", "email")]
    class RejectPolicyAdapter : ExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            // do nothing here in the demo
        }

    }
}
