using System;
using System.Collections.Generic;
using CamundaClient.Dto;
using CamundaClient.Worker;
using System.Net.Mail;
using System.Net;

namespace WebinarDemoWorker
{
    [ExternalTaskTopic("informCustomerCancellation")]
    [ExternalTaskVariableRequirements("email")]
    class InformCustomerCancellationAdapter : IExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            //String email = (string)externalTask.variables["email"].value;
            String email = "bernd.ruecker@camunda.com";

            /*
            MailMessage mail = new MailMessage("demo@camunda.com", email);
            SmtpClient client = new SmtpClient();

            //...
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.camunda.com";
            client.Credentials = new NetworkCredential("demo@mx.camunda.com", "28484234386345");
 
            mail.Subject = "Order Shipped";
            mail.Body = "Hooray";

            client.Send(mail);
            */ 

            Console.WriteLine();
            Console.WriteLine("Cancellation Mail sent!");
            Console.WriteLine();

        }

    }
}
