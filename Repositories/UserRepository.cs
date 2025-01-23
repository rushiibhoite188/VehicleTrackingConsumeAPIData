using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleTrackingApp.Data;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VehicleTrackingContext _context;

        public UserRepository(VehicleTrackingContext context)
        {
            _context = context;
        }

        public async Task CreateUser(UserMaster user)
        {
            await _context.UserMasters.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(UserMaster user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserMaster>> GetUsers()
        {
            return await _context.UserMasters.ToListAsync();
        }

        public async Task<UserMaster> GetUserById(int id)
        {
            return await _context.UserMasters.FindAsync(id);
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.UserMasters.FindAsync(id);
            if (user != null)
            {
                _context.UserMasters.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
