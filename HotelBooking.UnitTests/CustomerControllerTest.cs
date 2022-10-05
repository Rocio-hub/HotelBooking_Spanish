using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class CustomerControllerTest
    {
        private CustomersController controller;
        private Mock<IRepository<Customer>> fakeCustomerRepository;
        List<Customer> customers = new List<Customer>();

        public CustomerControllerTest()
        {
            customers = new List<Customer>
            {
                new Customer { Id=1, Name="CustomerA", Email="CustomerA@gmail.com" },
                new Customer { Id=2, Name="CustomerB", Email="CustomerB@gmail.com" },
            };

            // Create fake CustomerRepository. 
            fakeCustomerRepository = new Mock<IRepository<Customer>>();

            // Implement fake GetAll() method.
            fakeCustomerRepository.Setup(x => x.GetAll()).Returns(customers);


            // Implement fake Get() method.
            


            // Alternative setup with argument matchers:

            // Any integer:
            //fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(rooms[1]);

            // Integers from 1 to 2 (using a predicate)
            // If the fake Get is called with an another argument value than 1 or 2,
            // it returns null, which corresponds to the behavior of the real
            // repository's Get method.
            //fakeRoomRepository.Setup(x => x.Get(It.Is<int>(id => id > 0 && id < 3))).Returns(rooms[1]);

            // Integers from 1 to 2 (using a range)
            /*fakeCustomerRepository.Setup(x =>
            x.Get(It.IsInRange<int>(1, 2, Moq.Range.Inclusive))).Returns(customers[1]);
            */

            // Create CustomerController
            controller = new CustomersController(fakeCustomerRepository.Object);
        }
        [Fact]
        public void GetAll_ReturnsListWithCorrectNumberOfCustomers()
        {
            // Act
            var result = controller.Get() as List<Customer>;
            var noOfCustomers = result.Count;

            // Assert
            Assert.Equal(2, noOfCustomers);
        }
        [Theory]
        [InlineData(1,"CustomerA")]
        [InlineData(2,"CustomerB")]
        public void GetById_ReturnsCorrectCustomer(int id, string customerName)
        {
            // Act
            fakeCustomerRepository.Setup(x => x.Get(id)).Returns(customers[id-1]);
            var result = controller.GetById(id);

            // Assert
            Assert.Equal(customerName, result.Name);
        }
    }
}
