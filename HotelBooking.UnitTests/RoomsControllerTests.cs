using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class RoomsControllerTests
    {
        private RoomsController controller;
        private Mock<IRepository<Room>> fakeRoomRepository;
        public RoomsControllerTests()
        {
            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            // Create fake RoomRepository. 
            fakeRoomRepository = new Mock<IRepository<Room>>();

            // Implement fake GetAll() method.
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(rooms);


            // Implement fake Get() method.
            //fakeRoomRepository.Setup(x => x.Get(2)).Returns(rooms[1]);


            // Alternative setup with argument matchers:

            // Any integer:
            //fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(rooms[1]);

            // Integers from 1 to 2 (using a predicate)
            // If the fake Get is called with an another argument value than 1 or 2,
            // it returns null, which corresponds to the behavior of the real
            // repository's Get method.
            //fakeRoomRepository.Setup(x => x.Get(It.Is<int>(id => id > 0 && id < 3))).Returns(rooms[1]);

            // Integers from 1 to 2 (using a range)
            fakeRoomRepository.Setup(x =>
            x.Get(It.IsInRange<int>(1, 2, Moq.Range.Inclusive))).Returns(rooms[1]);


            // Create RoomsController
            controller = new RoomsController(fakeRoomRepository.Object);
        }

        [Theory]
        [InlineData(2)]
        public void GetAll_ReturnsListWithCorrectNumberOfRooms(int numberOfRoomsExpected)
        {
            // Act
            var result = controller.Get() as List<Room>;
            var noOfRooms = result.Count;

            // Assert
            Assert.Equal(numberOfRoomsExpected, noOfRooms);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 1, 2)]
        public void GetById_RoomExists_ReturnsIActionResultWithRoom(int id, int low, int high)
        {
            // Act
            var result = controller.Get(id) as ObjectResult;
            var room = result.Value as Room;
            var roomId = room.Id;


            // Assert
            Assert.InRange<int>(roomId, low, high);
        }

        [Theory]
        [InlineData(1)]
        public void Delete_WhenIdIsLargerThanZero_RemoveIsCalled(int idToDelete)
        {
            // Act
            controller.Delete(idToDelete);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(idToDelete), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Delete_WhenIdIsLessThanOne_RemoveIsNotCalled(int roomId)
        {
            // Act
            controller.Delete(roomId);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

        [Theory]
        [InlineData(3)]
        public void Delete_WhenIdIsLargerThanTwo_RemoveThrowsException(int roomIdDoesNotExist)
        {
            // Instruct the fake Remove method to throw an InvalidOperationException, if a room id that
            // does not exist in the repository is passed as a parameter. This behavior corresponds to
            // the behavior of the real repoository's Remove method.
            fakeRoomRepository.Setup(x =>
                    x.Remove(It.Is<int>(id => id < 1 || id > 2))).
                    Throws<InvalidOperationException>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => controller.Delete(roomIdDoesNotExist));

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }


    }
}
