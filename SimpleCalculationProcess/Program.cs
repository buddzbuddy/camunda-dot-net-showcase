using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camunda;

namespace SimpleCalculationProcess
{
    class Program
    {

        private static void Main(string[] args)
        {
            CamundaClient camunda = new CamundaClient();
            camunda.StartWorkers();

            var tasks = camunda.HumanTaskService().LoadTasks();
            foreach (var task in tasks)
            {
                Console.WriteLine(task.Name);
            }

            camunda.RepositoryService().Deploy();            

            Console.WriteLine("\n\nEnter um Fenster zu schließen.");
            Console.ReadLine();

            camunda.StopWorkers();
        }

    }
}
