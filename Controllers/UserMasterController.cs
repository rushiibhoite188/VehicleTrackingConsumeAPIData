using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VehicleTrackingApp.Models;

namespace VehicleTrackingApp.Controllers
{
    public class UserMasterController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserMasterController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7184/api/UserMaster/");
        }

        // GET: UserMaster/Index
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync("GetUsers");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<IEnumerable<UserMasters>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Implement pagination
                    var pagedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                    ViewBag.TotalPages = Math.Ceiling((double)users.Count() / pageSize);
                    ViewBag.CurrentPage = pageNumber;

                    return View(pagedUsers);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error fetching users: {ex.Message}");
            }

            return View("Error");
        }

        // GET request : UserMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST request for Create User: UserMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserMasters user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        var filePath = Path.Combine(uploadsFolder, file.FileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        user.PhotoPath = "/uploads/" + file.FileName;
                    }

                    var userJson = JsonSerializer.Serialize(user);
                    var content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("PostUser", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "User created successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create user. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating user: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input. Please check the form and try again.";
            }

            return View(user);
        }



        // GET Request for edit : UserMaster/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"GetUserById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserMasters>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error fetching user details: {ex.Message}");
            }

            return View("Error");
        }

        // POST: UserMaster/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserMasters user, IFormFile uploadedPhoto)
        {
            if (id != user.UserID)
            {
                return BadRequest("User ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadedPhoto != null && uploadedPhoto.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uploadedPhoto.FileName));

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedPhoto.CopyToAsync(stream);
                        }

                        user.PhotoPath = $"/uploads/{Path.GetFileName(uploadedPhoto.FileName)}";
                    }

                    var userJson = JsonSerializer.Serialize(user);
                    var content = new StringContent(userJson, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"PutUser/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "User updated successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var apiError = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"API error: {apiError}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                }
            }
            else
            {
                ModelState.AddModelError("", "Model is not valid. Please correct the errors and try again.");
            }

            return View(user);
        }


        // GET: UserMaster/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"GetUserById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserMasters>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error fetching user details: {ex.Message}");
            }

            return View("Error");
        }
        // POST: UserMaster/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"DeleteUser/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User deleted successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error deleting user: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            return RedirectToAction("Index");
        }

    }
}
