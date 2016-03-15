using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camunda;
using System.Threading;

namespace SimpleCalculationProcess
{
    [ExternalTaskAdapter("calculate")]
    class CalculationAdapter : Adapter
    {

        public void Execute(ExternalTask externalTask)
        {
            Console.WriteLine("Execute External Task " + externalTask);
            Thread.Sleep(10 * 1000); // 10 seconds
            Console.WriteLine("Finished " + externalTask);
        }


    }
}
