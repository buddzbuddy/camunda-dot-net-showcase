using CamundaClient.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

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
                var result = JsonConvert.DeserializeObject<IEnumerable<ProcessDefinition>>(
                    response.Content.ReadAsStringAsync().Result);
                http.Dispose();

                // Could be extracted into seperate method call if you run a lot of process definitions and want to optimize performance
                foreach (ProcessDefinition pd in result)
                {
                    http = helper.HttpClient("process-definition/" + pd.id + "/startForm");
                    HttpResponseMessage response2 = http.GetAsync("").Result;
                    var startForm = JsonConvert.DeserializeObject<StartFormDto>(
                        response2.Content.ReadAsStringAsync().Result);

                    pd.startFormKey = startForm.key;
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

        public void DeleteDeployment(string deploymentId)
        {
            HttpClient http = helper.HttpClient("deployment/" + deploymentId + "?cascade=true");
            HttpResponseMessage response = http.DeleteAsync("").Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = response.Content.ReadAsStringAsync();
                http.Dispose();
                throw new EngineException(response.ReasonPhrase);
            }
            http.Dispose();
        }

        public String Deploy(string deploymentName, List<object> files)
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("deployment-name", deploymentName);
            postParameters.Add("deployment-source", "C# Process Application");
            postParameters.Add("enable-duplicate-filtering", "true");
            postParameters.Add("data", files);

            // Create request and receive response
            string postURL = helper.RestUrl + "deployment/create";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, helper.RestUsername, helper.RestPassword, postParameters);

            using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                var deployment = JsonConvert.DeserializeObject<Deployment>(reader.ReadToEnd());
                return deployment.id;
            }            
        }

        public void AutoDeploy()
        {
            Assembly thisExe = Assembly.GetEntryAssembly();
            string[] resources = thisExe.GetManifestResourceNames();

            if (resources.Length == 0)
            {
                return;
            }

            List<object> files = new List<object>();
            foreach (string resource in resources)
            {
                // TODO Check if Camunda relevant (BPMN, DMN, HTML Forms)

                // Read and add to Form for Deployment                
                files.Add(FileParameter.fromManifestResource(thisExe, resource));

                Console.WriteLine("Adding resource to deployment: " + resource);
            }

            Deploy(thisExe.GetName().Name, files);

            Console.WriteLine("Deployment to Camunda BPM succeeded.");

        }

    }

    public class FileParameter
    {
        public byte[] File { get; }
        public string FileName { get; }
        public string ContentType { get; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }

        static public FileParameter fromManifestResource(Assembly assembly, string resourcePath)
        {
            Stream resourceAsStream = assembly.GetManifestResourceStream(resourcePath);
            byte[] resourceAsBytearray;
            using (MemoryStream ms = new MemoryStream())
            {
                resourceAsStream.CopyTo(ms);
                resourceAsBytearray = ms.ToArray();
            }

            // TODO: Verify if this is the correct way of doing it:
            string assemblyBaseName = assembly.GetName().Name;      
            string fileLocalName = resourcePath.Replace(assemblyBaseName + ".", "");

            return new FileParameter(resourceAsBytearray, fileLocalName);
        }
    }

    public class StartFormDto
    {
        public string key { get; set; }
    }

    /*
    * Basis taken from http://www.briangrinstead.com/blog/multipart-form-post-in-c
    */
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string username, string password, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, username, password, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string username, string password, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new EngineException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            //request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            if (username != null)
            {
                request.PreAuthenticate = true;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(username + ":" + password)));
            }

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
                requestStream.Dispose();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();

            // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
            // Skip it on the first parameter, add it to subsequent parameters.
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (param.Value is List<object>)
                {
                    // list of files
                    foreach (var value in (List < object >)param.Value)
                    {
                        if (needsCLRF)
                            formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                        addFormData(boundary, formDataStream, param.Key, value);
                        needsCLRF = true;
                    }
                } else {
                    // only a single file
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    addFormData(boundary, formDataStream, param.Key, param.Value);
                    needsCLRF = true;
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            formDataStream.Dispose();

            return formData;
        }

        private static void addFormData(string boundary, Stream formDataStream, String key, object value)
        {
            if (value is FileParameter)
            {
                FileParameter fileToUpload = (FileParameter)value;

                // Add just the first part of this param, since we will write the file data directly to the Stream
                string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                     fileToUpload.FileName ?? key,
                    fileToUpload.FileName ?? key,
                    fileToUpload.ContentType ?? "application/octet-stream");

                formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                // Write the file data directly to the Stream, rather than serializing it to a string.
                formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
            }
            else
            {
                string postData = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    key,
                    value);
                formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
            }            
        }
       
    }
}
