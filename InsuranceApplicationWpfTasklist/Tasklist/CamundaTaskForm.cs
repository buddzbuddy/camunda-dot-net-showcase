using Camunda;
using System.Collections.Generic;
using System.Windows.Controls;

namespace InsuranceApplicationWpfTasklist
{
    internal interface CamundaTaskForm
    {
        void initialize(TasklistWindow tasklist, HumanTask task);
    }
}