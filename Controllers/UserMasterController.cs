using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;
using VehicleTrackingApp.Repositories;

namespace VehicleTrackingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMasterController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserMasterController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMaster>>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserMaster>> PostUser(UserMaster user)
        {
            await _userRepository.CreateUser(user);
            return CreatedAtAction(nameof(PostUser), new { id = user.UserID }, user);
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserMaster user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            await _userRepository.UpdateUser(user);
            return NoContent();
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserMaster>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUser(id);
            return NoContent();
        }
    }
}
