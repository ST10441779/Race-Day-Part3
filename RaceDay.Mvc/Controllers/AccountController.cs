using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiClient _api;

    public AccountController(RaceDayApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _api.PostAsync<
                LoginViewModel, AuthResponse>(
                    "/api/auth/login", model);

            if (result == null)
            {
                ModelState.AddModelError(
                    "", "Login failed. Please try again.");

                return View(model);
            }

            HttpContext.Session.SetString(
                "UserId", result.Id.ToString());

            HttpContext.Session.SetString(
                "FullName", result.FullName);

            HttpContext.Session.SetString(
                "Email", result.Email);

            HttpContext.Session.SetString(
                "Role", result.Role);

            if (result.Role == "Organiser")
                return RedirectToAction(
                    "Dashboard", "Organiser");

            return RedirectToAction(
                "Index", "Participant");
        }
        catch
        {
            ModelState.AddModelError(
                "", "Unable to log in. Check your details and API connection.");

            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _api.PostAsync<
                RegisterViewModel, AuthResponse>(
                    "/api/auth/register", model);

            // Automatically log in after successful registration.
            var login = await _api.PostAsync<
                LoginViewModel, AuthResponse>(
                    "/api/auth/login",
                    new LoginViewModel
                    {
                        Email = model.Email,
                        Password = model.Password
                    });

            if (login == null)
                throw new InvalidOperationException(
                    "Account created, but automatic login failed.");

            HttpContext.Session.SetString(
                "UserId", login.Id.ToString());

            HttpContext.Session.SetString(
                "FullName", login.FullName);

            HttpContext.Session.SetString(
                "Email", login.Email);

            HttpContext.Session.SetString(
                "Role", login.Role);

            if (login.Role == "Organiser")
                return RedirectToAction(
                    "Dashboard", "Organiser");

            return RedirectToAction(
                "Index", "Participant");
        }
        catch
        {
            ModelState.AddModelError(
                "", "Registration failed. Check your information and try again.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _api.PostAsync<object, JsonElement>(
                "/api/auth/logout", new { });
        }
        catch
        {
            // Always clear the local session.
        }

        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    // VIEW PROFILE

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login");

        try
        {
            var profile = await _api.GetAsync<AuthResponse>("/api/profile");

            if (profile == null)
                return RedirectToAction("Login");

            return View(new ProfileViewModel
            {
                FullName = profile.FullName,
                Email = profile.Email,
                Role = profile.Role
            });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;

            return View(new ProfileViewModel
            {
                FullName = HttpContext.Session.GetString("FullName") ?? "",
                Email = HttpContext.Session.GetString("Email") ?? "",
                Role = HttpContext.Session.GetString("Role") ?? ""
            });
        }
    }

    // UPDATE PROFILE

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login");

        model.Role = HttpContext.Session.GetString("Role") ?? "";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new
            {
                FullName = model.FullName,
                Email = model.Email
            };

            var updated = await _api.PutAsync<object, AuthResponse>(
                "/api/profile", request);

            if (updated == null)
            {
                ModelState.AddModelError(
                    "",
                    "The profile could not be updated.");

                return View(model);
            }

            HttpContext.Session.SetString("FullName", updated.FullName);
            HttpContext.Session.SetString("Email", updated.Email);

            TempData["Success"] = "Your profile was updated successfully.";

            return RedirectToAction("Profile");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}