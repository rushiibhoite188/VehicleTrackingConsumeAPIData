using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Repositories
{
    public interface IUserVehicleRepository
    {
        // GET methods
        Task<IEnumerable<UserVehicleDetails>> GetAllVehicles();
        Task<UserVehicleDetails> GetVehicleById(string id);
        Task<IEnumerable<UserVehicleDetails>> SearchVehicles(string vehicleNumber, int pageNumber, int pageSize);

        // POST method
        Task CreateVehicle(UserVehicleDetails userVehicle);

        // PUT method
        Task UpdateVehicle(UserVehicleDetails userVehicle);

        // DELETE method
        Task DeleteVehicle(string id);
    }
}
