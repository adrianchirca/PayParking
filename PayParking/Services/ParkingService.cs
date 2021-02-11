using ConsoleTables;
using Microsoft.Extensions.Configuration;
using PayParking.Entities;
using PayParking.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PayParking.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IConfiguration _configuration;

        public ParkingService(IParkingRepository parkingRepository, IConfiguration configuration)
        {
            _parkingRepository = parkingRepository;
            _configuration = configuration;
        }

        public ParkingSpot AddNewParkingEntry(string registrationNumber)
        {
            var freeParkingSpots = _parkingRepository.GetNumberOfFreeParkingSpots().GetAwaiter().GetResult();
            if (freeParkingSpots == 0)
            {
                Console.WriteLine("We are sorry! There are no avaiable parking places at the moment.\n");
                return null;
            }

            if (!IsRegistrationNumberValid(registrationNumber))
            {
                Console.WriteLine("Wrong format of registration number! Please try again.\n");
                return null;
            }

            if (_parkingRepository.GetParkingCarByRegistrationNumber(registrationNumber).GetAwaiter().GetResult() != null)
            {
                Console.WriteLine("There is already a car with this registration number. Please try again. \n");
                return null;
            }

            var parkingSpot = _parkingRepository.AddNewParkingEntry(registrationNumber).GetAwaiter().GetResult();
            Console.WriteLine($"You have left your car on number {parkingSpot.ParkingSpotNumber}.\n");

            return parkingSpot;
        }

        public Car ExitTheParking(string registrationNumber)
        {
            var car = _parkingRepository.ExitTheParking(registrationNumber)?.GetAwaiter().GetResult();

            if (car == null)
            {
                Console.WriteLine($"It seems like there is no car parked identified by the registration number you introduced. Please try again!\n");
                return null;
            }

            GenerateClientSummary(car);
            return car;
        }

        public int GetNumberOfFreeParkingSpots()
        {
            var freeParkingSpots = _parkingRepository.GetNumberOfFreeParkingSpots().GetAwaiter().GetResult();

            if (freeParkingSpots == 0)
                Console.WriteLine("There are no avaiable parking places at the moment.\n");
            else
                Console.WriteLine($"There are {freeParkingSpots} parking place(s) avaiable at the moment.\n");

            return (int)freeParkingSpots;
        }

        public List<Car> GetParkedCars()
        {
            var parkedCars = _parkingRepository.GetParkedCars().GetAwaiter().GetResult();
            ConsoleTable parkedCarsTable;

            if (!parkedCars.Any())
            {
                parkedCarsTable = new ConsoleTable("There is no car parked");
                parkedCarsTable.Write(Format.Alternative);
                return null;
            }

            parkedCarsTable = new ConsoleTable("OrderNo.", "Registration number", "Entrance time", "Period");
            parkedCars.ForEach(x =>
            {
                parkedCarsTable.AddRow(parkedCars.IndexOf(x), x.RegistrationNumber, x.EntranceTime, 10);
            });
            parkedCarsTable.Write(Format.Alternative);

            return parkedCars;
        }

        public void DisplayCosts()
        {
            decimal firstHourFee = GetFirstHourFee();
            decimal regularHourFee = GetRegularHourFee();

            Console.WriteLine($"First hour of stay is {firstHourFee} RON, then {regularHourFee} RON per hour.\n");
        }

        #region InternalMethods
        private bool IsRegistrationNumberValid(string registrationNumber)
        {
            return Regex.IsMatch(registrationNumber, "^[A-Za-z]{1,2}[0-9]{2,3}[A-Za-z]{3}");
        }

        private void GenerateClientSummary(Car clientCar)
        {
            Console.WriteLine($"Thank you for using our service!");
            var summary = new ConsoleTable("Registration number", "Entrance time", "Exit time", "Period", "Total amount")
              .AddRow(
                clientCar.RegistrationNumber,
                clientCar.EntranceTime,
                clientCar.ExitTime,
                clientCar.StationaryPeriod.Value.ToString("hh\\:mm\\:ss"),
                $"{CalculateTotalCost(clientCar.StationaryPeriod.Value)} RON");

            summary.Write(Format.Alternative);
        }

        private decimal CalculateTotalCost(TimeSpan stationaryPeriod)
        {
            decimal firstHourFee = GetFirstHourFee();
            decimal regularHourFee = GetRegularHourFee();

            var totalHours = (decimal)Math.Round(stationaryPeriod.TotalMinutes / 60, 0, MidpointRounding.ToPositiveInfinity);

            var totalCost = firstHourFee + ((totalHours - 1) * regularHourFee);
            return totalCost;
        }

        private decimal GetFirstHourFee()
        {
            if (!_configuration.GetSection("ParkingFees").GetSection("FirstHourFee").Exists())
                throw new Exception($"Configuration section ParkingFees.FirstHourFee was not found");

            decimal firstHourFee = _configuration.GetSection("ParkingFees").GetValue<decimal>("FirstHourFee");

            return firstHourFee;
        }

        private decimal GetRegularHourFee()
        {
            if (!_configuration.GetSection("ParkingFees").GetSection("RegularHourFee").Exists())
                throw new Exception($"Configuration section ParkingFees.RegularHourFee was not found");

            decimal regularHourFee = _configuration.GetSection("ParkingFees").GetValue<decimal>("RegularHourFee");

            return regularHourFee;
        }
        #endregion
    }
}
