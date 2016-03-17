using Camunda;
using System.Collections.Generic;
using System.Windows.Controls;

namespace InsuranceApplicationWpfTasklist
{
    internal interface CamundaStartForm
    {
        void initialize(CamundaClient Camunda, ProcessDefinition processDefinition);
    }
}