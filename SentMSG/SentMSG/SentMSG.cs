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

            Guid SMSid = context.PrimaryEntityId;
            string SMSType = context.PrimaryEntityName;
            ColumnSet attribute = new ColumnSet(new string[] { "new_phone_number_recipient", "new_message", "new_messageid" });

            Entity SMSEntity = service.Retrieve(SMSType, SMSid, attribute);

            SMSEntity.Attributes["new_messageid"] = (new Random().Next(0, 10000).ToString());
            Regex regexObj = new Regex(@"[^\d]");
            string correctNumber = ("+" + (string)regexObj.Replace((string)SMSEntity.Attributes["new_phone_number_recipient"], ""));
            
            Boolean checkWrite = false;
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(@"\\CRM-TRAIN\Shared\SMS.xml");

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SMS");

                xmlWriter.WriteStartElement("MessageId");
                xmlWriter.WriteString($"{SMSEntity.Attributes["new_messageid"]}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Phonenumber");
                xmlWriter.WriteString($"{correctNumber}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteString($"{SMSEntity.Attributes["new_message"]}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                checkWrite = true;
            }
            catch (Exception e)
            {
                throw new InvalidWorkflowException(e.Message.ToString());
            }
            if (checkWrite)
                service.Update(SMSEntity);
        }
    }
}



