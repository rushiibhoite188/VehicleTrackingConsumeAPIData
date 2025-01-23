using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTrackingApp.Data;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Repositories
{
    public class UserVehicleRepository : IUserVehicleRepository
    {
        private readonly VehicleTrackingContext _context;

        public UserVehicleRepository(VehicleTrackingContext context)
        {
            _context = context;
        }

        public async Task<UserVehicleDetails> GetVehicleById(string vehicleNumber)
        {
            return await _context.UserVehicleDetails.FindAsync(vehicleNumber);
        }

        public async Task<IEnumerable<UserVehicleDetails>> GetAllVehicles()
        {
            return await _context.UserVehicleDetails.ToListAsync();
        }

        public async Task CreateVehicle(UserVehicleDetails userVehicle)
        {
            // Check if the user exists in UserMasters
            var existingUser = await _context.UserMasters
                .FirstOrDefaultAsync(u => u.Email == userVehicle.UserMaster.Email);

            if (existingUser == null)
            {
                // Insert into UserMasters and get the generated UserID
                var newUser = new UserMaster
                {
                    Name = userVehicle.UserMaster.Name,
                    MobileNumber = userVehicle.UserMaster.MobileNumber,
                    Organization = userVehicle.UserMaster.Organization,
                    Address = userVehicle.UserMaster.Address,
                    Email = userVehicle.UserMaster.Email,
                    Location = userVehicle.UserMaster.Location,
                    PhotoPath = userVehicle.UserMaster.PhotoPath
                };
                _context.UserMasters.Add(newUser);
                await _context.SaveChangesAsync();

                userVehicle.UserID = newUser.UserID;
            }
            else
            {
                userVehicle.UserID = existingUser.UserID;
            }

            // Insert into UserVehicleDetails
            _context.UserVehicleDetails.Add(userVehicle);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateVehicle(UserVehicleDetails userVehicle)
        {
            var existingVehicle = await _context.UserVehicleDetails
                .Include(v => v.UserMaster) // Include related entities
                .FirstOrDefaultAsync(v => v.VehicleNumber == userVehicle.VehicleNumber);

            if (existingVehicle != null)
            {
                _context.Entry(existingVehicle).CurrentValues.SetValues(userVehicle);

                if (userVehicle.UserMaster != null)
                {
                    _context.Entry(existingVehicle.UserMaster).CurrentValues.SetValues(userVehicle.UserMaster);
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteVehicle(string vehicleNumber)
        {
            var vehicle = await GetVehicleById(vehicleNumber);
            if (vehicle != null)
            {
                _context.UserVehicleDetails.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserVehicleDetails>> SearchVehicles(string vehicleNumber)
        {
            return await _context.UserVehicleDetails
                .Where(v => v.VehicleNumber.Contains(vehicleNumber))
                .ToListAsync();
        }

        public async Task<IEnumerable<UserVehicleDetails>> SearchVehicles(string vehicleNumber, int pageNumber, int pageSize)
        {
            return await _context.UserVehicleDetails
                .Where(v => v.VehicleNumber.Contains(vehicleNumber))
                .Skip((pageNumber - 1) * pageSize )
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
