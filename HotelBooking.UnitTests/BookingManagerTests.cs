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

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [Fact]
        public void CreateBooking_ResultNotFalse()
        {
            // Arrange
            Booking booking = new Booking();
            DateTime startDate = new DateTime(2023, 10, 1);
            DateTime endDate = new DateTime(2023, 10, 3);

            booking.StartDate = startDate;
            booking.EndDate = endDate;
            booking.CustomerId = 1;
            booking.RoomId = 1;

            // Act
            bool isCreated = bookingManager.CreateBooking(booking);
            // Assert
            Assert.True(isCreated);
        }

        [Fact]
        public void CreateBooking_PastDate_ThrowsArgumentException()
        {
            // Arrange
            Booking booking = new Booking();
            DateTime startDate = new DateTime(2021, 10, 1);
            DateTime endDate = new DateTime(2021, 10, 3);

            booking.StartDate = startDate;
            booking.EndDate = endDate;
            booking.CustomerId = 1;
            booking.RoomId = 1;

            // Act
            Action act = () => bookingManager.CreateBooking(booking);
            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        public void CreateBooking_3Oct1Oct_ThrowsArgumentException()
        {
            // Arrange
            Booking booking = new Booking();
            DateTime startDate = new DateTime(2021, 10, 1);
            DateTime endDate = new DateTime(2021, 10, 3);

            booking.StartDate = endDate;
            booking.EndDate = startDate;
            booking.CustomerId = 1;
            booking.RoomId = 1;

            // Act
            Action act = () => bookingManager.CreateBooking(booking);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]


        [Fact]        
        public void GetFullyOccupiedDates_5octAnd6oct_ReturnEmpty()
        {
           // Arrange
           DateTime startDate = new DateTime(2022,10,05);
           DateTime endDate = startDate.AddDays(1);
           // Act
           List<DateTime> occupied = bookingManager.GetFullyOccupiedDates(startDate, endDate);
           // Assert
           Assert.Equal(0,occupied.Count);
        }

        [Fact]
        public void GetFullyOccupiedDates_9octAnd19oct_ReturnsFive()
        {
            // Arrange
            DateTime startDate = new DateTime(2022, 10, 09);
            DateTime endDate = startDate.AddDays(10);
            // Act
            List<DateTime> occupied = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equal(5, occupied.Count);
        }

        [Fact]
        public void GetFullyOccupiedDates_19octAnd9oct_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = new DateTime(2022, 10, 09);
            DateTime endDate = startDate.AddDays(10);
            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(endDate, startDate);
            // Assert
            Assert.Throws<ArgumentException>(act);
        }
    }
}
