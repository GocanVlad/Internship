using Acme.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Acme.CommonTest
{
    [TestClass]
    public class StringHandlerTest
    {
        [TestMethod]
        public void InsertSpacesTestValid()
        {
            var source = "SonicScrewdriver";
            var expected = "Sonic Screwdriver";

            var acutal = source.InsertSpaces();

            Assert.AreEqual(expected, acutal);
        }
    

        [TestMethod] 
        public void InsertSpacesTestWithExistingSpaces()
        { 
            var source = "Sonic Screwdriver";
            var expected = "Sonic Screwdriver";

            var acutal = source.InsertSpaces();

            Assert.AreEqual(expected, acutal);
        }
    }
}