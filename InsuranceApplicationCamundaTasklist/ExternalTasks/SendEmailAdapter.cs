using System;
using System.Collections.Generic;
using Camunda;
using System.Net.Mail;
using System.Net;

namespace InsuranceApplicationWpfTasklist
{
    [ExternalTaskTopic("sendEmail")]
    [ExternalTaskVariableRequirements("name", "carType", "carManufacturer", "email")]
    class SendEmailAdapter : ExternalTaskAdapter
    {

        public void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables)
        {
            string email = (string)externalTask.variables["email"].value;

            MailMessage mail = new MailMessage("demo@camunda.com", email);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mail.camunda.com";
            client.Credentials = new NetworkCredential("demo@mx.camunda.com", "28484234386345");
            mail.Subject = "Ihre Versicherungspolice.";
            // todo
            mail.Body = "this is my test email body";
            client.Send(mail);
        }

    }
}
