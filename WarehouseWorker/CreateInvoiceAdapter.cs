using System;
using System.Collections.Generic;
using CamundaClient.Dto;
using CamundaClient.Worker;

namespace WebinarDemoWorker
{
    [ExternalTaskTopic("createInvoice")]
    [ExternalTaskVariableRequirements("order")]
    class CreateInvoiceAdapter : IExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            Console.WriteLine();
            Console.WriteLine("Now a new invoice was created!");
            Console.WriteLine();
        }

    }
}
