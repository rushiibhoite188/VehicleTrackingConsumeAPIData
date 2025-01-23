using Microsoft.EntityFrameworkCore;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Data
{
    public class VehicleTrackingContext : DbContext
    {
        public VehicleTrackingContext(DbContextOptions<VehicleTrackingContext> options) : base(options) { }

        public DbSet<UserMaster> UserMasters { get; set; }
        public DbSet<UserVehicleDetails> UserVehicleDetails { get; set; }
    }
}
