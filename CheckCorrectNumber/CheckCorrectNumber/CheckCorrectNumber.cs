﻿using Microsoft.Xrm.Sdk;
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

                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity)) return;
                else
                {
                    Entity sms = (Entity)context.InputParameters["Target"];    //GetAttributeValue 
                    if (sms.Contains("new_phone_number_recipient") && !String.IsNullOrEmpty((string)sms.Attributes["new_phone_number_recipient"]))
                    {
                        String phonenumber = (string)sms.Attributes["new_phone_number_recipient"];
                        Regex regexObj = new Regex(@"[^\d]");
                        if ((!phonenumber.Substring(0, 2).Equals("+7")) || regexObj.Replace(phonenumber, "").Length != 11)
                        {
                            throw new InvalidPluginExecutionException("Sorry phone number incorrect. \n The phone number must be 11 digits and begin with +7");
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
