using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamundaClient;

namespace WebinarDemoWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            CamundaEngineClient camunda = new CamundaEngineClient();
            camunda.Startup(); // Deploys all models to Camunda and Start all found ExternalTask-Workers
            Console.ReadLine(); // wait for ANY KEY
            camunda.Shutdown(); // Stop Task Workers
        }
    }
}
