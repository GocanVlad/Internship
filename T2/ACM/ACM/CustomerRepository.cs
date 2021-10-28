using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACM.BL
{
    public class CustomerRepository
    {
        public CustomerRepository()
        {
            addressRepository = new AddressRepository();
        }
        private AddressRepository addressRepository { get; set; }

        //Retrieve one customer
        public Customer Retrieve(int customerId)
        {
            Customer customer = new Customer(customerId);


            if(customerId == 1)
            {
                customer.EmailAddress = "vgocan@yahoo.com";
                customer.FirstName = "Vlad";
                customer.LastName = "Gocan";
                customer.AddressList = addressRepository.RetireveByCustomerId(customerId).
                                                        ToList();
            }

            return customer;
        }

        public bool Save(Customer customer)
        {
            return true; 
        }
    }
}
