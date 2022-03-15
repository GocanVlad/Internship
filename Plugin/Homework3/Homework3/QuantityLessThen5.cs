using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework3
{
    public class QuantityLessThen5:IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (!context.InputParameters.ContainsKey("Target"))
                throw new InvalidPluginExecutionException("No target found");
            var entity = context.InputParameters["Target"] as Entity;

            int quantity = entity.GetAttributeValue<int>("aug_quantity");
            if (quantity < 5)
                throw new InvalidPluginExecutionException("Valoarea trebuie sa fie mai mare de 5");

        }
    }
}
