using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.UnitTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingControllerTests
    {
        private BookingsController controller;
        private Mock<IRepository<Booking>> fakeBookingRepository;
        private Mock<IRepository<Room>> fakeRoomRepository;
        private Mock<IRepository<Customer>> fakeCustomerRepository;
        private IBookingManager bookingManager;

        public BookingControllerTests()
        {
            DateTime startDate = new DateTime(2023, 10, 1);
            DateTime endDate = new DateTime(2023, 10, 3);
            var bookings = new List<Booking>
            {
                new Booking { Id=1, StartDate=startDate, EndDate=endDate, CustomerId=1, RoomId=1},
                new Booking { Id=2, StartDate=endDate, EndDate=endDate, CustomerId=2, RoomId=2},
            };

            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            var customers = new List<Customer>
            {
                new Customer { Id=1, Name="A", Email="A@A.com" },
                new Customer { Id=2, Name="B", Email="B@B.com" }
            };

            // Create fake BookingRepository. 
            fakeBookingRepository = new Mock<IRepository<Booking>>();

            // Implement fake GetAll() method.
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(bookings);

            // Create fake RoomRepository. 
            fakeRoomRepository = new Mock<IRepository<Room>>();

            // Implement fake GetAll() method.
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(rooms);

            // Create fake CustomerRepository. 
            fakeCustomerRepository = new Mock<IRepository<Customer>>();

            // Implement fake GetAll() method.
            fakeCustomerRepository.Setup(x => x.GetAll()).Returns(customers);

            // Alternative setup with argument matchers:

            // Any integer:
            //fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(rooms[1]);

            // Integers from 1 to 2 (using a predicate)
            // If the fake Get is called with an another argument value than 1 or 2,
            // it returns null, which corresponds to the behavior of the real
            // repository's Get method.
            //fakeRoomRepository.Setup(x => x.Get(It.Is<int>(id => id > 0 && id < 3))).Returns(rooms[1]);
            bookingManager = new BookingManager(fakeBookingRepository.Object, fakeRoomRepository.Object);
            // Integers from 1 to 2 (using a range)
            fakeBookingRepository.Setup(x =>
            x.Get(It.IsInRange<int>(1, 2, Moq.Range.Inclusive))).Returns(bookings[1]);


            // Create RoomsController
            controller = new BookingsController(fakeBookingRepository.Object, fakeRoomRepository.Object, fakeCustomerRepository.Object, bookingManager);
        }

        [Theory]
        [InlineData(2)]
        public void GetAll_ReturnsListWithCorrectNumberOfBookings(int numberOfBookingsExpected)
        {
            // Act
            var result = controller.Get() as List<Booking>;
            var noOfBookings = result.Count;

            // Assert
            Assert.Equal(numberOfBookingsExpected, noOfBookings);
        }

        [Theory]
        [InlineData(2, 1, 2)]
        [InlineData(1, 1, 2)]
        public void GetById_BookingExists_ReturnsFromRange(int id, int low, int high)
        {
            // Act
            var result = controller.Get(id) as ObjectResult;
            var booking = result.Value as Booking;
            var bookingId = booking.Id;

            // Assert
            Assert.InRange<int>(bookingId, low, high);
        }

        [Theory]
        [InlineData(2, 1, 2)]
        [InlineData(1, 1, 2)]
        public void GetById_BookingExists_ReturnsId(int id, int low, int high)
        {
            // Act
            fakeBookingRepository.Setup(x => x.Get(1)).Returns(bookings[0]);

            // Assert
            Assert.InRange<int>(bookingId, low, high);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1)]
        public void Delete_WhenIdIsLargerThanZero_RemoveIsCalled(int id)
        {
            // Act
            controller.Delete(id);

            // Assert against the mock object
            fakeBookingRepository.Verify(x => x.Remove(id), Times.Once);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void Delete_WhenIdIsLessThanOne_RemoveIsNotCalled(int id)
        {
            // Act
            controller.Delete(id);

            // Assert against the mock object
            fakeBookingRepository.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

        [Theory]
        [InlineData(20)]
        [InlineData(10)]
        [InlineData(5)]
        public void Delete_WhenIdIsLargerThanTwo_RemoveThrowsException(int bookingId)
        {
            // Instruct the fake Remove method to throw an InvalidOperationException, if a room id that
            // does not exist in the repository is passed as a parameter. This behavior corresponds to
            // the behavior of the real repoository's Remove method.
            fakeBookingRepository.Setup(x =>
                    x.Remove(It.Is<int>(id => id < 1 || id > 2))).
                    Throws<InvalidOperationException>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => controller.Delete(bookingId));

            // Assert against the mock object
            fakeBookingRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }
    }
}
