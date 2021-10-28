using ACM.BL;
using Acme.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ACM.Test
{
    [TestClass]
    public class LoggingServiceTest
    {
        [TestMethod]
        public void WriteToFileTest()
        {
            var changedItems = new List<ILoggable>();

            var custoer = new Customer(1)
            {
                EmailAddress = "vgocan@yahoo.com",
                FirstName = "Vlad",
                LastName = "Gocan",
                AddressList = null
            };
            changedItems.Add(custoer);

            var product = new Product(2)
            {
                ProductName = "Rake",
                ProductDescription = "Garden Rake with Steel Head",
                CurrentPrice = 6M
            };
            changedItems.Add(product);

            //ACT
            LoggingService.WriteToFile(changedItems);


        }
    }
}
