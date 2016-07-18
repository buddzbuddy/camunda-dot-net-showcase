using CamundaClient.Dto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CamundaClient
{
    public class CamundaClientHelper
    {
        public string RestUrl { get; }
        public string RestUsername { get; }
        public string RestPassword { get; }

        public CamundaClientHelper(string restUrl, string username, string password)
        {
            this.RestUrl = restUrl;
            this.RestUsername = username;
            this.RestPassword = password;
        }

        public HttpClient HttpClient(string path)
        {
            HttpClient client = null;
            if (RestUsername != null)
            {
                var credentials = new NetworkCredential(RestUsername, RestPassword);
                client = new HttpClient(new HttpClientHandler() { Credentials = credentials });
            }
            else
            {
                client = new HttpClient();
            }
            client.BaseAddress = new Uri(RestUrl + path);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public Dictionary<string, Variable> convertVariables(Dictionary<string, object> variables)
        {
            // report successfull execution
            Dictionary<string, Variable> result = new Dictionary<string, Variable>();
            foreach (var variable in variables)
            {
                Variable camundaVariable = new Variable();
                camundaVariable.value = variable.Value;
                result.Add(variable.Key, camundaVariable);
            }
            return result;
        }
    }
}
