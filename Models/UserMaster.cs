using System.ComponentModel.DataAnnotations;

namespace VehicleTrackingApp.Models
{
    public class UserMaster
    {
        [Key]
        public int UserID { get; set; }

        public string? Name { get; set; }

        [Phone]
        public string? MobileNumber { get; set; }

        public string? Organization { get; set; }

        public string? Address { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Location { get; set; }

        public string? PhotoPath { get; set; }
    }
}
