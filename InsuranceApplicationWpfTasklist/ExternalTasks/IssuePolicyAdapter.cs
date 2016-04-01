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
            // just create an id for demo purposes here
            resultVariables.Add("policyId", Guid.NewGuid().ToString());
        }

    }
}
