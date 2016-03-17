using System;
using System.Collections.Generic;
using Camunda;

namespace InsuranceApplicationWpfTasklist
{
    [ExternalTaskTopic("issuePolicy")]
    [ExternalTaskVariableRequirements("name", "carType", "carManufacturer", "email")]
    class IssuePolicyAdapter : ExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
        }

    }
}
