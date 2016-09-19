using CamundaClient;
using CamundaClient.Service;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleCalculationProcess
{
    [TestFixture]
    class CalculationProcessTest
    {
        [Test]
        public void TestHappyPath()
        {
            // Engine client should point to a dedicated Camunda instance for test, preferrably locally available
            var camunda = new CamundaEngineClient("http://localhost:8080/engine-rest/engine/default/", null, null);

            // Deploy the process under test
            string deploymentId = camunda.RepositoryService().Deploy("testcase", new List<object> {
                FileParameter.fromManifestResource(Assembly.GetExecutingAssembly(), "SimpleCalculationProcess.calculation.bpmn") });

            camunda.BpmnWorkflowService().StartProcessInstance("calculate", new Dictionary<string, object>()
            {
                {"x", 5 },
                {"y", 10 }
            });

            var externalTasks = camunda.ExternalTaskService().FetchAndLockTasks("testcase", 100, "calculate", 1000, new List<string>() { "x", "y" });
            Assert.AreEqual(1, externalTasks.Count);
            Assert.AreEqual("ServiceTaskCalculate", externalTasks.First().activityId);

            camunda.ExternalTaskService().Complete("testcase", externalTasks.First().id, new Dictionary<string, object>() { { "result", 15 } });

            var tasks = camunda.HumanTaskService().LoadTasks();
            //Assert.AreEqual(1, tasks.Count);
            //Assert.AreEqual("ServiceTaskCalculate", tasks.First().activityId);

            //camunda.HumanTaskService().Complete(

            // not the process instance has ended, TODO: Check state with History

            // cleanup after test case
            camunda.RepositoryService().DeleteDeployment(deploymentId);
        }
    }
}
