using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(UserMaster user);
        Task UpdateUser(UserMaster user);
        Task<IEnumerable<UserMaster>> GetUsers();
        Task<UserMaster> GetUserById(int id);
        Task DeleteUser(int id);
    }
}
