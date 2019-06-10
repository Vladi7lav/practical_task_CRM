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

namespace SentMSG
{
    public sealed class SentMSG : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {    

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            //ITracingService tracer = executionContext.GetExtension<ITracingService>();

            Guid smsId = context.PrimaryEntityId;
            string smsType = context.PrimaryEntityName;
            ColumnSet attribute = new ColumnSet(new string[] { "new_phone_number_recipient", "new_message", "new_messageid", "statuscode", "statecode"});

            Entity smsEntity = service.Retrieve(smsType, smsId, attribute);

            smsEntity.Attributes["new_messageid"] = (new Random().Next(100000, 999999).ToString());

            Regex regexObj = new Regex(@"[^\d]");
            string number = (string)smsEntity.Attributes["new_phone_number_recipient"];
            string correctNumber = "";
            string[] phonenumbersArray = Regex.Split(number, "\\s*;\\s*");
            foreach (var num in phonenumbersArray)
            {
                correctNumber += (String.IsNullOrEmpty(num) ? "" : "+" + (string)regexObj.Replace(num, "") + "; ");
            }
            tracer.Trace("Last view: " + correctNumber);
            Boolean checkWrite = false;
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(@"\\CRM-TRAIN\Shared\SMS.xml");

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SMS");

                xmlWriter.WriteStartElement("MessageId");
                xmlWriter.WriteString($"{smsEntity.Attributes["new_messageid"]}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Phonenumber");
                xmlWriter.WriteString($"{correctNumber}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteString($"{smsEntity.Attributes["new_message"]}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                checkWrite = true;
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
            if (!checkWrite) return;
            else
            {
                smsEntity.GetAttributeValue<OptionSetValue>("statecode").Value = 1;
                smsEntity.GetAttributeValue<OptionSetValue>("statuscode").Value = 100000002;
                service.Update(smsEntity);
            }               
        }
    }
}



