using Microsoft.Extensions.Configuration;
using PayParking.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayParking.Repositories
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly IConfiguration _configuration;
        private readonly List<ParkingSpot> _parkingSpots;
        private readonly List<Car> _cars;

        public ParkingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            if (!_configuration.GetSection("TotalNumberOfParkingSpots").Exists())
                throw new Exception($"Configuration section TotalNumberOfParkingSpots");

            var totalNumberOfParkingSpots = _configuration.GetValue<int>("TotalNumberOfParkingSpots");

            if (!_configuration.GetSection("ParkingSpotFormatNumber").Exists())
                throw new Exception($"Configuration section ParkingSpotFormatNumber");

            var parkingSpotFormatNumber = _configuration.GetValue<string>("ParkingSpotFormatNumber");

            _cars = new List<Car>();
            _parkingSpots = new List<ParkingSpot>();

            for (var i = 1; i <= totalNumberOfParkingSpots; i++)
            {
                _parkingSpots.Add(new ParkingSpot()
                {
                    ParkingSpotId = i,
                    ParkingSpotNumber = parkingSpotFormatNumber.Replace("{id}", i.ToString()),
                    CarId = null,
                    Car = null
                });
            }
        }

        public Task<ParkingSpot> AddNewParkingEntry(string registrationNumber)
        {
            var newEntryCar = new Car() { Id = (_cars.Count + 1), RegistrationNumber = registrationNumber.ToUpper(), EntranceTime = DateTime.Now };
            _cars.Add(newEntryCar);
            var parkingSpot = _parkingSpots.FirstOrDefault(x => x.CarId == null);
            parkingSpot.CarId = newEntryCar.Id;
            parkingSpot.Car = newEntryCar;

            return Task.FromResult(parkingSpot);

        }

        public Task<Car> ExitTheParking(string registrationNumber)
        {
            var parkingSpot = _parkingSpots.FirstOrDefault(x => x?.Car?.RegistrationNumber?.ToUpper() == registrationNumber.ToUpper());
            if (parkingSpot == null) return null;

            parkingSpot.Car = null;
            parkingSpot.CarId = null;

            var car = _cars.OrderByDescending(x => x.EntranceTime).FirstOrDefault(x => x.RegistrationNumber.ToUpper() == registrationNumber.ToUpper());
            car.ExitTime = DateTime.Now;
            return Task.FromResult(car);
        }

        public Task<int> GetNumberOfFreeParkingSpots()
        {
            return Task.FromResult(_parkingSpots.Where(x => x.CarId == null).ToList().Count);
        }

        public Task<List<Car>> GetParkedCars()
        {
            return Task.FromResult(_parkingSpots.Where(x => x.CarId != null).Select(x => x.Car).ToList());
        }

        public Task<Car> GetParkingCarByRegistrationNumber(string registrationNumber)
        {
            var car = _parkingSpots.FirstOrDefault(x => x?.Car?.RegistrationNumber.ToUpper() == registrationNumber.ToUpper());
            return Task.FromResult(car?.Car);
        }
    }
}
