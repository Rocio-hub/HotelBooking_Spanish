using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;

        public BookingManagerTests()
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        public static IEnumerable<object[]> GetLocalData_FindAvailableRoom()
        {
            var data = new List<object[]>
            {
                new object[] {DateTime.Today.AddDays(2), -1, false}
            };

            return data;
        }

        [Theory]
        [InlineData("2023,10,01", "2023,10,03", 1, 1, true)]
        public void CreateBooking_ValidInlineData_BookingIsCreated(DateTime startDate, DateTime endDate, int customerId, int roomId, bool expectedResult)
        {
            //Arrange
            Booking booking = new Booking();
            // string startD = startDate.ToString();
            // string endD = endDate.ToString();

            booking.StartDate = startDate;
            booking.EndDate = endDate;
            booking.CustomerId = customerId;
            booking.RoomId = roomId;

            //Act
            bool isCreated = bookingManager.CreateBooking(booking);
            //Assert
            Assert.True(isCreated);
        }

        [Theory]
        [InlineData("2021,10,01", "2021,10,03", 1, 1, typeof(ArgumentException), "The start date cannot be in the past or later than the end date.")]
        public void FindAvailableRoom_ValidInlineData_ThrowsException(DateTime startDate, DateTime endDate, int customerId, int roomId, Type exceptionType, string message)
        {
            //Arrange
            Booking booking = new Booking();

            booking.StartDate = startDate;
            booking.EndDate = endDate;
            booking.CustomerId = customerId;
            booking.RoomId = roomId;
            //Act
            try
            {
                bool isCreated = bookingManager.CreateBooking(booking);
            }
            catch (Exception e)
            {
                //Assert
                Assert.True(e.GetType() == exceptionType);
                Assert.Equal(e.Message, message);
            }
        }


        [Theory]
        [MemberData(nameof(GetLocalData_FindAvailableRoom))]
        public void FindAvailableRoom_ValidMemberData_RoomIdPositive(DateTime date, int roomId, bool expectedResult)
        {
            //Act
            var actualResult = roomId > 0;
            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("2022-10-05", "2022-10-09", 0)]
        [InlineData("2022-10-09", "2022-10-19", 5)]
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
            try
            {
                List<DateTime> list = bookingManager.GetFullyOccupiedDates(endDate, startDate);
            }
            catch (Exception e)
            {
                // Assert
                Assert.True(e.GetType() == exceptionType);
                Assert.Equal(e.Message, message);
            }
        }
    }
}
