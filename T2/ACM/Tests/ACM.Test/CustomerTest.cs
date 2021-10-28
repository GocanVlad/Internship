using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ACM.BL;

namespace ACM.BLTest
{
    [TestClass]
    public class CustomerTest
    {
        [TestMethod]
        public void FullNameTestValue()
        {
            //-- Arrange
            Customer customer = new Customer()
            {
                FirstName = "Vlad",
                LastName = "Gocan"
            };
            string expected = "Gocan, Vlad";

            //-- Act
            string actual = customer.FullName;

            //-- Assert 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FullNameFirstNameEmpty()
        {
            //-- Arrange
            Customer customer = new Customer()
            {
                LastName = "Gocan"
            };
            string expected = "Gocan";

            //-- Act
            string actual = customer.FullName;

            //-- Assert 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FullNameLastNameEmpty()
        { 
            //-- Arrange
            Customer customer = new Customer()
            {
                FirstName = "Vlad"
            };
            string expected = "Vlad";

            //-- Act
            string actual = customer.FullName;

            //-- Assert 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void StaticTest()
        {
            //--Arrange
            var c1 = new Customer();
            c1.FirstName = "Vlad";
            Customer.InstanceCount++;

            var c2 = new Customer();
            c2.FirstName = "Dan";
            Customer.InstanceCount++;

            var c3 = new Customer();
            c3.FirstName = "Alex";
            Customer.InstanceCount++;

            //--Act

            //Assert
            Assert.AreEqual(3, Customer.InstanceCount);
        }

        [TestMethod]
        public void ValidateValid()
        {
            var customer = new Customer()
            {
                LastName = "Gocan",
                EmailAddress = "vgocan@yahoo.com"
            };
            var expected = true;

            var actual = customer.Validate();

            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void ValidateMissingLastName()
        {
            var customer = new Customer()
            {
                EmailAddress = "vgocan@yahoo.com"
            };
            var expected = false;

            var actual = customer.Validate();

            Assert.AreEqual(expected, actual);
        }

    }
}
