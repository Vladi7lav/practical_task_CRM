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

namespace SentMSG
{
    public sealed class SentMSG : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            //System.Diagnostics.Debugger.Launch();            
            EntityReference SMSref = this.SMS.Get(executionContext);

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            Entity SMSEntity;

            RetrieveRequest retrieveRequest = new RetrieveRequest();
            retrieveRequest.ColumnSet = new ColumnSet(new string[] { "new_phone_number_recipient", "new_message" });
            retrieveRequest.Target = SMSref;
            RetrieveResponse retrieveResponse = (RetrieveResponse)service.Execute(retrieveRequest);
            SMSEntity = retrieveResponse.Entity as Entity;

            SMSEntity.Attributes["new_messageid"] = new Random().Next(0, 100);
            SMSEntity.Attributes["serviceid"] = SMSref;
            service.Update(SMSEntity);

            XmlWriter xmlWriter = XmlWriter.Create(@"C:\SMS.xml");

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("SMS");

            xmlWriter.WriteStartElement("Id message");
            xmlWriter.WriteString($"{SMSEntity.Attributes["id"]}");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Phonenumber");
            xmlWriter.WriteString($"{SMSEntity.Attributes["new_phone_number_recipient"]}");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Message");
            xmlWriter.WriteString($"{SMSEntity.Attributes["new_message"]}");
            xmlWriter.WriteEndElement();            

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        [RequiredArgument]
        [Input("Sent MSG")]
        [ReferenceTarget("new_sms")]
        public InArgument<EntityReference> SMS { get; set; }
    }
}



