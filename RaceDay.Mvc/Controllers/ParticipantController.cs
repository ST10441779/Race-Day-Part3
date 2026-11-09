using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _api;

    public ParticipantController(RaceDayApiClient api)
    {
        _api = api;
    }

    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("Role")
            == "Participant";
    }

    private IActionResult RequireParticipant()
    {
        if (string.IsNullOrEmpty(
            HttpContext.Session.GetString("UserId")))
            return RedirectToAction("Login", "Account");

        if (!IsParticipant())
            return RedirectToAction("Dashboard", "Organiser");

        return null!;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _api.GetAsync<
            List<RaceEventViewModel>>("/api/events");

        return View(events ?? new());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var item = await _api.GetAsync<
            RaceEventViewModel>($"/api/events/{id}");

        if (item == null)
            return NotFound();

        var categories = await _api.GetAsync<
            List<CategoryViewModel>>(
                $"/api/events/{id}/categories");

        var model = new EventDetailsViewModel
        {
            Event = item,
            Categories = categories ?? new()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(
        EnrolmentViewModel model)
    {
        var access = RequireParticipant();

        if (access != null)
            return access;

        if (!ModelState.IsValid)
            return RedirectToAction(
                "Details", new { id = model.EventId });

        try
        {
            await _api.PostAsync<
                EnrolmentViewModel, JsonElement>(
                    "/api/enrolments", model);

            TempData["Success"] =
                "You have successfully enrolled in this event.";

            return RedirectToAction("Index");
        }
        catch
        {
            TempData["Error"] =
                "Enrolment failed. You may already be enrolled.";

            return RedirectToAction(
                "Details", new { id = model.EventId });
        }
    }

    public async Task<IActionResult> MyResults()
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        try
        {
            var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>(
                "/api/enrolments/mine");

            var results = await _api.GetAsync<List<RaceResultViewModel>>(
                "/api/results/mine");

            var events = await _api.GetAsync<List<RaceEventViewModel>>(
                "/api/events");

            var myEnrolments = enrolments ?? new List<EnrolmentViewModel>();
            var myResults = results ?? new List<RaceResultViewModel>();
            var allEvents = events ?? new List<RaceEventViewModel>();

            var output = new List<ParticipantResultViewModel>();

            foreach (var result in myResults)
            {
                var enrolment = myEnrolments.FirstOrDefault(
                    x => x.Id == result.EnrolmentId);

                if (enrolment == null)
                    continue;

                var raceEvent = allEvents.FirstOrDefault(
                    x => x.Id == enrolment.EventId);

                if (raceEvent == null)
                    continue;

                var categories = await _api.GetAsync<List<CategoryViewModel>>(
                    $"/api/events/{enrolment.EventId}/categories");

                var categoryName = categories?
                    .FirstOrDefault(x => x.Id == enrolment.CategoryId)?.Name
                    ?? "Category unavailable";

                output.Add(new ParticipantResultViewModel
                {
                    EventName = raceEvent.Name,
                    EventDate = raceEvent.Date,
                    CategoryName = categoryName,
                    FinishTime = result.FinishTime,
                    Position = result.Position
                });
            }

            return View(output.OrderBy(x => x.EventDate).ToList());
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(new List<ParticipantResultViewModel>());
        }
    }
}