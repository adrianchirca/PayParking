using Microsoft.Extensions.Configuration;
using Moq;
using PayParking.Entities;
using PayParking.Repositories;
using PayParking.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PayParking.Tests
{
    public class ParkingServiceTests
    {
        private readonly ParkingService _parkingService;
        private readonly Mock<IParkingRepository> _parkingRepositoryMock = new Mock<IParkingRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        public ParkingServiceTests()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"ParkingSpotFormatNumber", "{id}PP"},
                {"TotalNumberOfParkingSpots", "10" },
                {"ParkingFees:FirstHourFee", "10"},
                {"ParkingFees:RegularHourFee", "5"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _parkingService = new ParkingService(_parkingRepositoryMock.Object, configuration);
        }

        [Fact]
        public void GetNumberOfFreeParkingSpots_ShouldReturnIntValue()
        {
            //Arrange
            var result = 10;
            _parkingRepositoryMock.Setup(x => x.GetNumberOfFreeParkingSpots()).Returns(Task.FromResult(result));

            //Act
            var serviceExpectedResult = _parkingService.GetNumberOfFreeParkingSpots();

            //Assert
            Assert.Equal(result, serviceExpectedResult);
        }

        [Theory]
        [InlineData("B01ABC")]
        public void AddNewParkingEntry_ShouldReturnParkingSpotInstance(string registrationNumber)
        {
            //Arrange
            ParkingSpot parkingSpot = new ParkingSpot()
            {
                ParkingSpotId = 1,
                ParkingSpotNumber = "1PP",
                CarId = 1,
                Car = new Car()
                {
                    Id = 1,
                    EntranceTime = DateTime.Now,
                    ExitTime = DateTime.Now,
                    RegistrationNumber = registrationNumber
                }
            };
            _parkingRepositoryMock.Setup(x => x.AddNewParkingEntry(registrationNumber)).Returns(Task.FromResult(parkingSpot));
            _parkingRepositoryMock.Setup(x => x.GetNumberOfFreeParkingSpots()).Returns(Task.FromResult(10));


            //Act
            ParkingSpot parkingSpotExpected = _parkingService.AddNewParkingEntry(registrationNumber);

            //Assert
            Assert.Equal(registrationNumber, parkingSpotExpected.Car.RegistrationNumber);
        }

        [Theory]
        [InlineData("B01ABC")]
        public void ExitTheParking_ShouldReturnCar(string registrationNumber)
        {
            //Arrange
            Car car = new Car()
            {
                Id = 1,
                EntranceTime = DateTime.Now,
                ExitTime = DateTime.Now,
                RegistrationNumber = registrationNumber
            };
            _parkingRepositoryMock.Setup(x => x.ExitTheParking(registrationNumber)).Returns(Task.FromResult(car));

            //Act
            Car carExpected = _parkingService.ExitTheParking(registrationNumber);

            //Assert
            Assert.Equal(registrationNumber, carExpected.RegistrationNumber);
        }
    }


}
