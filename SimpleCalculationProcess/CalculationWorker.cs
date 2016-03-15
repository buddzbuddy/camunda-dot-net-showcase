using System;
using System.Collections.Generic;
using Camunda;

namespace SimpleCalculationProcess
{
    [ExternalTaskAdapter("calculate")]
    [ExternalTaskVariableRequirements("x", "y")]
    class CalculationAdapter : Adapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            Console.WriteLine("Execute External Task " + externalTask);

            long x = Convert.ToInt64(externalTask.variables["x"].value);
            long y = Convert.ToInt64(externalTask.variables["y"].value);
            long result = x + y;
            resultVariables.Add("result", result);

            Console.WriteLine("Finished External Task " + externalTask);
        }


    }
}
