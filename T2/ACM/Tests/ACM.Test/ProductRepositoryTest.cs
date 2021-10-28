using ACM.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACM.Test
{
    [TestClass]
    public class ProductRepositoryTest
    {
        [TestMethod]
        public void RetrieveTest()
        {
            var productRepository = new ProductRepository();
            var expected = new Product(2)
            {
                CurrentPrice = 15.96M,
                ProductDescription = "Assorted Size Set of 4 Sunflowers",
                ProductName = "Sunflowers"
            };

            var acutal = productRepository.Retrieve(2);

            Assert.AreEqual(expected.CurrentPrice, acutal.CurrentPrice);
            Assert.AreEqual(expected.ProductDescription, acutal.ProductDescription);
            Assert.AreEqual(expected.ProductName, acutal.ProductName);
        }

        [TestMethod]
        public void SaveTestValid()
        {
            var productRepository = new ProductRepository();
            var updatedProduct = new Product(2)
            {
                CurrentPrice = 18M,
                ProductDescription = "Assorted Size Set of 4 Yellow Sunflowers",
                ProductName = "Sunflowers",
                HasChanges = true
            };

            var actual = productRepository.Save(updatedProduct);

            Assert.AreEqual(true, actual);
        }


        [TestMethod]
        public void SaveTestMissingPrice()
        {
            var productRepository = new ProductRepository();
            var updatedProduct = new Product(2)
            {
                CurrentPrice = null,
                ProductDescription = "Assorted Size Set of 4 Yellow Sunflowers",
                ProductName = "Sunflowers",
                HasChanges = true
            };

            var actual = productRepository.Save(updatedProduct);

            Assert.AreEqual(false, actual);
        }


    }
}
