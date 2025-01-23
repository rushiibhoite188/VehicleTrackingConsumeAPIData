using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;
using VehicleTrackingApp.Repositories;

namespace VehicleTrackingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVehicleDetailsController : ControllerBase
    {
        private readonly IUserVehicleRepository _userVehicleRepository;

        public UserVehicleDetailsController(IUserVehicleRepository userVehicleRepository)
        {
            _userVehicleRepository = userVehicleRepository;
        }


        // GET: api/UserVehicleDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserVehicleDetails>>> GetAllUserVehicles()
        {
            var vehicles = await _userVehicleRepository.GetAllVehicles();
            return Ok(vehicles);
        }

        // POST: api/UserVehicleDetails
        [HttpPost]
        public async Task<ActionResult<UserVehicleDetails>> PostUserVehicle([FromBody] UserVehicleDetails userVehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userVehicle == null)
            {
                return BadRequest("User vehicle details cannot be null.");
            }

            try
            {
                await _userVehicleRepository.CreateVehicle(userVehicle);

                return CreatedAtAction(nameof(GetUserVehicleById),
                  new { VehicleNumber = userVehicle.VehicleNumber }, userVehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/UserVehicleDetails/{VehicleNumber}
        [HttpPut("{VehicleNumber}")]
        public async Task<IActionResult> PutUserVehicle(string VehicleNumber, UserVehicleDetails userVehicle)
        {
            if (userVehicle == null)
            {
                return BadRequest("Vehicle details cannot be null.");
            }

            if (VehicleNumber != userVehicle.VehicleNumber)
            {
                return BadRequest("Vehicle number in the URL does not match the body.");
            }

            var existingVehicle = await _userVehicleRepository.GetVehicleById(VehicleNumber);
            if (existingVehicle == null)
            {
                return NotFound($"Vehicle with Vehicle Number {VehicleNumber} not found.");
            }

            try
            {
                // Update the vehicle
                await _userVehicleRepository.UpdateVehicle(userVehicle);

                return Ok("Vehicle details updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating vehicle: {ex.Message}");
            }
        }


        // DELETE: api/UserVehicleDetails/{id}
        [HttpDelete("{VehicleNumber}")]
        public async Task<IActionResult> DeleteUserVehicle(string VehicleNumber)
        {
            var vehicle = await _userVehicleRepository.GetVehicleById(VehicleNumber);

            if (vehicle == null)
            {
                return NotFound($"Vehicle with Vehicle Number {VehicleNumber} not found.");
            }

            await _userVehicleRepository.DeleteVehicle(VehicleNumber);
            return NoContent();
        }

        // GET: api/UserVehicleDetails/{id}
        [HttpGet("{VehicleNumber}")]
        public async Task<ActionResult<UserVehicleDetails>> GetUserVehicleById(string VehicleNumber)
        {
            var vehicle = await _userVehicleRepository.GetVehicleById(VehicleNumber);

            if (vehicle == null)
            {
                return NotFound($"Vehicle with Vehicle Number {VehicleNumber} not found.");
            }

            return Ok(vehicle);
        }

        // GET: api/UserVehicleDetails/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserVehicleDetails>>> GetUserVehicles(
            [FromQuery] string vehicleNumber,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var vehicles = await _userVehicleRepository.SearchVehicles(vehicleNumber, pageNumber, pageSize);
            return Ok(vehicles);
        }
    }
}
