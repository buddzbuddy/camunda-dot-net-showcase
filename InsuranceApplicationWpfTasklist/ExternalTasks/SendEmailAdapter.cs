using System;
using System.Collections.Generic;
using Camunda;

namespace InsuranceApplicationWpfTasklist
{
    [ExternalTaskTopic("sendEmail")]
    [ExternalTaskVariableRequirements("name", "carType", "carManufacturer", "email")]
    class SendEmailAdapter : ExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
        }

    }
}
