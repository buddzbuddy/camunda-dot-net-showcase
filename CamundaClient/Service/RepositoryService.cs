using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using CamundaClient.Requests;

namespace CamundaClient.Service
{

    public class RepositoryService
    {
        private CamundaClientHelper helper;

        public RepositoryService(CamundaClientHelper helper)
        {
            this.helper = helper;
        }


        public List<ProcessDefinition> LoadProcessDefinitions(bool onlyLatest)
        {
            var http = helper.HttpClient("process-definition/");
            HttpResponseMessage response = http.GetAsync("?latestVersion=" + (onlyLatest ? "true" : "false")).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsAsync<IEnumerable<ProcessDefinition>>().Result;
                http.Dispose();

                // Could be extracted into separate method call if you run a lot of process definitions and want to optimize performance
                foreach (ProcessDefinition pd in result)
                {
                    http = helper.HttpClient("process-definition/" + pd.Id + "/startForm");
                    HttpResponseMessage response2 = http.GetAsync("").Result;
                    var startForm = response2.Content.ReadAsAsync<StartForm>().Result;
                    pd.StartFormKey = startForm.Key;
                    http.Dispose();
                }
                return new List<ProcessDefinition>(result);
            }
            else
            {
                http.Dispose();
                return new List<ProcessDefinition>();
            }

        }

        public void AutoDeploy()
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetEntryAssembly();
            string[] resources = thisExe.GetManifestResourceNames();

            if (resources.Length == 0)
            {
                return;
            }

            // TODO: Verify if this is the correct way of doing it:
            String assemblyBaseName = thisExe.GetName().Name;

            List<object> files = new List<object>();
            foreach (string resource in resources)
            {
                // TODO Check if Camunda relevant (BPMN, DMN, HTML Forms)

                // Read and add to Form for Deployment
                Stream resourceAsStream = thisExe.GetManifestResourceStream(resource);
                byte[] resourceAsBytearray;
                using (MemoryStream ms = new MemoryStream())
                {
                    resourceAsStream.CopyTo(ms);
                    resourceAsBytearray = ms.ToArray();
                }

                String fileLocalName = resource.Replace(assemblyBaseName + ".", "");
                files.Add(new FileParameter(resourceAsBytearray, fileLocalName));

                Console.WriteLine("Adding resource to deployment: " + resource);
            }
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("deployment-name", assemblyBaseName);
            postParameters.Add("deployment-source", "C# Process Application");
            postParameters.Add("enable-duplicate-filtering", "true");
            postParameters.Add("data", files);

            // Create request and receive response
            string postURL = helper.RestUrl + "deployment/create";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, helper.RestUsername, helper.RestPassword, postParameters);

            Console.WriteLine($"Deployment to Camunda BPM succeeded. {webResponse.StatusCode}");

        }

    }
}
