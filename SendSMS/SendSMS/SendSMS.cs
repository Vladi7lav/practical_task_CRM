using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using System.Xml;
using System.Text.RegularExpressions;

namespace SendSMS
{
    public sealed class SendSMS : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            string id = new Random().Next(100000, 999999).ToString();
            smsId.Set(executionContext, id);
            string message = (string)messageText.Get(executionContext);
            string number = (string)phoneNumber.Get(executionContext);
            string[] phonenumbersArray = Regex.Split(number, "\\s*;\\s*");
            number = "";

            Regex regexObj = new Regex(@"[^\d]");

            foreach (var num in phonenumbersArray)
            {
                number += (String.IsNullOrEmpty(num) ? "" : "+" + (string)regexObj.Replace(num, "") + "; ");
            }
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(@"\\CRM-TRAIN\Shared\newSMS.xml");

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SMS");

                xmlWriter.WriteStartElement("smsID");
                xmlWriter.WriteString($"{id}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Phonenumber");
                xmlWriter.WriteString($"{number}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteString($"{message}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
        }
        [Input("Phone number(s)")]
        public InArgument<string> phoneNumber { get; set; }
        [Input("Message text")]
        public InArgument<string> messageText { get; set; }
        [Output("smsID")]
        public OutArgument<string> smsId { get; set; }
    }
}