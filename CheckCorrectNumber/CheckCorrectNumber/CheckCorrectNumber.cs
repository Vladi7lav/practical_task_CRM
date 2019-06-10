using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckCorrectNumber
{
    public class CheckCorrectNumber : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var service = serviceFactory.CreateOrganizationService(context.UserId);
                                
                Entity sms = (Entity)context.InputParameters["Target"]; 
                String phonenumbers = sms.GetAttributeValue<string>("new_phone_number_recipient");

                if (String.IsNullOrEmpty(phonenumbers)) return;
                else
                {
                    string[] phonenumbersArray = Regex.Split(phonenumbers, "\\s*;\\s*");
                    foreach (var phonenumber in phonenumbersArray)
                    {
                        Regex notD = new Regex(@"[^\d]");
                        if (!String.IsNullOrEmpty(phonenumber) && ((!phonenumber.Substring(0, 2).Equals("+7")) || (notD.Replace(phonenumber, "")).Length != 11))
                        {
                            throw new InvalidPluginExecutionException("Sorry phone number " + phonenumber + " incorrect. \n The phone number must be 11 digits and begin with +7");
                        }
                    }                    
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                throw e;
            }
        }
    }
}
