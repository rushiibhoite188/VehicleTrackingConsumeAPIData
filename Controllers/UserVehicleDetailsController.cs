using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VehicleTrackingApp.Data;

namespace VehicleTrackingApp.Controllers
{
    public class UserVehicleDetailsController : Controller
    {
        private readonly HttpClient _httpClient;
        public UserVehicleDetailsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7184/api/UserVehicleDetails/");
        }

        // GET: UserVehicleDetails
        public async Task<IActionResult> Index(string searchVehicleNumber = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync("GetAllUserVehicles");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vehicles = JsonSerializer.Deserialize<IEnumerable<UserVehicleDetailss>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (vehicles == null || !vehicles.Any())
                    {
                        ViewBag.TotalPages = 0;
                        ViewBag.CurrentPage = 1;
                        return View(new List<UserVehicleDetailss>());
                    }

                    // Filter by searchVehicleNumber if provided
                    if (!string.IsNullOrEmpty(searchVehicleNumber))
                    {
                        vehicles = vehicles.Where(v => v.VehicleNumber.Equals(searchVehicleNumber, StringComparison.OrdinalIgnoreCase));
                        if (!vehicles.Any())
                        {
                            ViewBag.TotalPages = 0;
                            ViewBag.CurrentPage = 1;
                            ViewBag.ErrorMessage = "Vehicle number not found.";
                            return View(new List<UserVehicleDetailss>());
                        }
                    }

                    // Pagination logic
                    var pagedVehicles = vehicles.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                    ViewBag.TotalPages = Math.Ceiling((double)vehicles.Count() / pageSize);
                    ViewBag.CurrentPage = pageNumber;

                    return View(pagedVehicles);
                }
                else
                {
                    ModelState.AddModelError("", $"Error fetching data: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }

            ViewBag.TotalPages = 0;
            ViewBag.CurrentPage = 1;
            return View(new List<UserVehicleDetailss>());
        }

        // GET: UserVehicleDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVehicleDetailss vehicle)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Validation failed. Please check your input.");
                return View(vehicle);
            }

            try
            {
                var vehicleJson = JsonSerializer.Serialize(vehicle);
                var content = new StringContent(vehicleJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("PostUserVehicle", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", $"API Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception: {ex.Message}");
            }

            return View(vehicle);
        }



        // GET: UserVehicleDetails/Edit/{VehicleNumber}
        public async Task<IActionResult> Edit(string vehicleNumber)
        {
            if (string.IsNullOrEmpty(vehicleNumber))
                return BadRequest("Vehicle number is required.");

            try
            {
                var response = await _httpClient.GetAsync($"GetUserVehicleById/{vehicleNumber}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vehicle = JsonSerializer.Deserialize<UserVehicleDetailss>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(vehicle);
                }

                ModelState.AddModelError("", $"Error fetching vehicle details: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return RedirectToAction("Index");
        }

        // POST: UserVehicleDetails/Edit/{VehicleNumber}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string vehicleNumber, UserVehicleDetailss vehicle)
        {
            if (vehicleNumber != vehicle.VehicleNumber)
                return BadRequest("Vehicle number mismatch.");

            if (ModelState.IsValid)
            {
                try
                {
                    var vehicleJson = JsonSerializer.Serialize(vehicle);
                    var content = new StringContent(vehicleJson, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"PutUserVehicle/{vehicleNumber}", content);

                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index");

                    ModelState.AddModelError("", $"Error updating vehicle: {response.ReasonPhrase}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
                }
            }
            return View(vehicle);
        }

        // GET: UserVehicleDetails/Delete/{VehicleNumber}
        public async Task<IActionResult> Delete(string vehicleNumber)
        {
            if (string.IsNullOrEmpty(vehicleNumber))
                return BadRequest("Vehicle number is required.");

            try
            {
                var response = await _httpClient.GetAsync($"GetUserVehicleById/{vehicleNumber}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vehicle = JsonSerializer.Deserialize<UserVehicleDetailss>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(vehicle);
                }

                ModelState.AddModelError("", $"Error fetching vehicle details: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return RedirectToAction("Index");
        }

        // POST: UserVehicleDetails/Delete/{VehicleNumber}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string vehicleNumber)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"DeleteUserVehicle/{vehicleNumber}");
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", $"Error deleting vehicle: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return RedirectToAction("Index");
        }
    }
}