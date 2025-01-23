using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleTrackingApp.Models
{
    public class UserVehicleDetails
    {
        [Key]
        public string? VehicleNumber { get; set; }

        public string? VehicleType { get; set; }

        public string? ChassisNumber { get; set; }

        public string? EngineNumber { get; set; }

        public int? ManufacturingYear { get; set; }

        public double? LoadCarryingCapacity { get; set; }

        public string? Make { get; set; }

        public string? ModelNumber { get; set; }

        public string? BodyType { get; set; }

        public string? OrganizationName { get; set; }

        public string? DeviceID { get; set; }

        [ForeignKey("UserMaster")]
        public int UserID { get; set; }
            
        public UserMaster? UserMaster { get; set; }
    }
}
