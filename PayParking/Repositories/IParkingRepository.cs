using PayParking.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayParking.Repositories
{
    public interface IParkingRepository
    {
        Task<int> GetNumberOfFreeParkingSpots();
        Task<List<Car>> GetParkedCars();
        Task<ParkingSpot> AddNewParkingEntry(string registrationNumber);
        Task<Car> ExitTheParking(string registrationNumber);
        Task<Car> GetParkingCarByRegistrationNumber(string registrationNumber);
    }
}
