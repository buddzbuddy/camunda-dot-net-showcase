using System;
using System.Collections.Generic;
using Camunda;

namespace SimpleCalculationProcess
{
    [ExternalTaskTopic("calculate")]
    [ExternalTaskVariableRequirements("x", "y")]
    class CalculationAdapter : ExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            long x = Convert.ToInt64(externalTask.variables["x"].value);
            long y = Convert.ToInt64(externalTask.variables["y"].value);
            long result = x + y;
            resultVariables.Add("result", result);
        }

    }
}
