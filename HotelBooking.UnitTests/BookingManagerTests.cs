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
            _mockBookingRepository = new Mock<IRepository<Booking>>();
            _mockRoomRepository = new Mock<IRepository<Room>>();

            //DateTime start = DateTime.Today.AddDays(10);
            //DateTime end = DateTime.Today.AddDays(20);

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
                EndDate = DateTime.Today.AddDays(7),
                IsActive = false,
                Customer = customer2,
                CustomerId = customer2.Id,
                RoomId = roomList[1].Id,
                Room = roomList[1]
            });

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
        public void FindAvailableRoom_ValidData_RoomIdNotMinusOne()
        {
            //Arrange
            var startDate = new DateTime(2022, 10, 8);
            var endDate = new DateTime(2022, 10, 9);
            //bookingList[1].IsActive = true;
            //Act
            var roomId = bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public void FindAvailableRoom_ValidData_ThrowsException()
        {
            //Arrange
            Booking booking = new Booking();
            booking.StartDate = new DateTime(2021, 10, 07);
            booking.EndDate = new DateTime(2021, 10, 08);
            booking.CustomerId = 1;
            booking.RoomId = 1;
            //Act
            Action act = () => bookingManager.CreateBooking(booking);
            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [MemberData(nameof(GetLocalData_GetFullyOccupiedDates))]
        public void GetFullyOccupiedDates_ValidMemberData_RightNumberOfFullyOccupiedDates(DateTime startDate, DateTime endDate, int expectedResult)
        {
            // Act
            bookingList[1].IsActive = true;
            List<DateTime> occupied = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equal(expectedResult, occupied.Count);
        }

        public static IEnumerable<object[]> GetLocalData_GetFullyOccupiedDates()
        {
            var data = new List<object[]>
            {
                new object[] {DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), 1},
                new object[] {DateTime.Today.AddDays(2), DateTime.Today.AddDays(7), 1},
                new object[] {DateTime.Today.AddDays(8), DateTime.Today.AddDays(9), 0}
            };
            return data;
        }

        [Fact]
        public void GetFullyOccupiedDates_ValidInlineData_ThrowsException()
        {
            // Arrange
            var startDate = new DateTime(2022, 10, 20);
            var endDate = new DateTime(2022, 10, 10);
            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Throws<ArgumentException>(act);
        }
    }
}