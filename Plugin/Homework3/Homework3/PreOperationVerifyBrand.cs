using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.Text.RegularExpressions;

namespace Homework3
{
    public class PreOperationVerifyBrand : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
           IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (!context.InputParameters.ContainsKey("Target"))
                throw new InvalidPluginExecutionException("No target found");
            var entity = context.InputParameters["Target"] as Entity;

            if (!entity.Attributes.Contains("aug_brand"))
            {
                entity["aug_brand"] = "Nu a fost specificat vreun brand";
            }
        }
    }
}
