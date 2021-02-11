using PayParking.Entities;
using System.Collections.Generic;

namespace PayParking.Services
{
    public interface IParkingService
    {
        int GetNumberOfFreeParkingSpots();
        ParkingSpot AddNewParkingEntry(string registrationNumber);
        Car ExitTheParking(string registrationNumber);
        List<Car> GetParkedCars();
        void DisplayCosts();

    }
}
