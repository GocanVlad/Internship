using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Homework3
{
    public class OptionSetCreateChildRecord : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.

                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId); 
                
                if (entity.LogicalName != "account")
                    return;

                try
                {            
                    Entity followup = new Entity("aug_tshirt");
                    followup.Attributes.Add("aug_name", "TT" +" "+ DateTime.UtcNow);
                    followup.Attributes.Add("aug_client",new EntityReference("account",entity.Id));
                    followup.Attributes.Add("aug_size",new OptionSetValue(751710000));
                    followup.Attributes.Add("aug_color",new OptionSetValue(751710000));
                    followup.Attributes.Add("aug_quantity", 6);
                    followup.Attributes.Add("aug_price",new Money(100));
                    followup.Attributes.Add("aug_tva",(double)25);
                    
                    var optionSetForCreate = entity.GetAttributeValue<bool>("aug_createchildrecord");
                    if (entity.Attributes.Contains("aug_createchildrecord") && optionSetForCreate)
                    {
                        service.Create(followup);
                        entity["aug_createchildrecord"]= false;
                        service.Update(entity);
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin. "+ ex.Message);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
