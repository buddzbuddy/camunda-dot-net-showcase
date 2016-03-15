using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Camunda
{

    public class HumanTaskService
    {
        private CamundaClient client;

        public HumanTaskService(CamundaClient client)
        {
            this.client = client;
        }

        public IList<HumanTask> LoadTasks()
        {
            HttpClient http = client.HttpClient("task/");

            HttpResponseMessage response = http.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Successful - parse the response body
                var tasks = response.Content.ReadAsAsync<IEnumerable<HumanTask>>().Result;
                return new List<HumanTask>(tasks);
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return new List<HumanTask>();
            }

        }
    }


}
