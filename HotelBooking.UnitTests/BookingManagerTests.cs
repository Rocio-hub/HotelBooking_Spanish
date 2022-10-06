using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private readonly Mock<IRepository<Booking>> _mockBookingRepository;
        private readonly Mock<IRepository<Room>> _mockRoomRepository;

        List<Room> roomList = new List<Room>();
        List<Booking> bookingList = new List<Booking>();

        public BookingManagerTests()
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);

            Customer customer1 = new Customer();
            customer1.Name = "Customer1";
            customer1.Email = "customer1@email";

            Customer customer2 = new Customer();
            customer2.Name = "Customer2";
            customer2.Email = "customer2@email";

            roomList.Add(new Room()
            {
                Id = 1,
                Description = "Room number one"
            });
            roomList.Add(new Room()
            {
                Id = 2,
                Description = "Room number two"
            });

            bookingList.Add(new Booking()
            {
                Id = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                IsActive = true,
                Customer = customer1,
                CustomerId = customer1.Id,
                RoomId = roomList[0].Id,
                Room = roomList[0]
            });

            bookingList.Add(new Booking()
            {
                Id = 2,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(3),
                IsActive = false,
                Customer = customer2,
                CustomerId = customer2.Id,
                RoomId = roomList[1].Id,
                Room = roomList[1]
            });

            _mockBookingRepository = new Mock<IRepository<Booking>>();
            _mockRoomRepository = new Mock<IRepository<Room>>();

            //Setup
            _mockRoomRepository.Setup(x => x.GetAll()).Returns(roomList);
            _mockBookingRepository.Setup(x => x.GetAll()).Returns(bookingList);
            _mockBookingRepository.Setup(x => x.Add(It.IsAny<Booking>())).Callback<Booking>((s) => bookingList.Add(s));

            bookingManager = new BookingManager(_mockBookingRepository.Object, _mockRoomRepository.Object);
        }

        [Fact]
        public void CreateBooking_ValidDates_isCreatedIsTrue()
        {
            //Arrange
            Booking booking = new Booking();
            booking.StartDate = DateTime.Today.AddDays(1);
            booking.EndDate = booking.StartDate.AddDays(2);
            booking.CustomerId = 1;
            booking.RoomId = 1;
            //Act
            bool isCreated = bookingManager.CreateBooking(booking);
            //Assert
            Assert.True(isCreated);
        }

        [Fact]      
        public void FindAvailableRoom_ValidData_ThrowsException()
        {
            //Arrange
            Booking booking = new Booking();
            booking.StartDate = new DateTime(2021,10,06);
            booking.EndDate = new DateTime(2021, 10, 07);
            booking.CustomerId = 1;
            booking.RoomId = 1;
            //Act
            Action act = () => bookingManager.CreateBooking(booking);
            //Assert
            Assert.Throws<ArgumentException>(act);
        }
                
        [Fact]
        public void FindAvailableRoom_ValidData_RoomIdPositive()
        {
            //Arrange
            var startDate = new DateTime(2022, 10, 7);
            var endDate = new DateTime(2022, 10, 8);
            //bookingList[1].IsActive = true;
            //Act
            var roomId = bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        /*
        [Theory]
        [InlineData("2022-10-05", "2022-10-09", 0)]
        [InlineData("2022-10-09", "2022-10-19", 4)]
        public void GetFullyOccupiedDates_ValidMemberData(DateTime startDate, DateTime endDate, int expectedResult)
        {
            // Act
            List<DateTime> occupied = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equal(expectedResult, occupied.Count);
        }

        [Theory]
        [InlineData("2022-10-09", "2022-10-19", typeof(ArgumentException), "The start date cannot be later than the end date.")]
        public void GetFullyOccupiedDates_ValidInlineData_ThrowsException(DateTime startDate, DateTime endDate, Type exceptionType, string message)
        {
            // Arrange
            string startD = startDate.ToString();
            string endD = endDate.ToString();
            // Act
            {
                List<DateTime> list = bookingManager.GetFullyOccupiedDates(endDate, startDate);
                // Assert
                //  Assert.True(e.GetType() == exceptionType);
                //  Assert.Equal(e.Message, message);
            }
        }*/

    }
}