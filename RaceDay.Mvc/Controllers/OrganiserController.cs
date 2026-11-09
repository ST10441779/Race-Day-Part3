using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController : Controller
{
    private readonly RaceDayApiClient _api;

    public OrganiserController(RaceDayApiClient api)
    {
        _api = api;
    }

    private bool IsOrganiser()
    {
        return !string.IsNullOrEmpty(
                   HttpContext.Session.GetString("UserId"))
               && HttpContext.Session.GetString("Role") == "Organiser";
    }

    private IActionResult? CheckAccess()
    {
        if (string.IsNullOrEmpty(
            HttpContext.Session.GetString("UserId")))
            return RedirectToAction("Login", "Account");

        if (!IsOrganiser())
            return RedirectToAction("Index", "Participant");

        return null;
    }

    private async Task<List<RaceEventViewModel>> MyEvents()
    {
        var events = await _api.GetAsync<
            List<RaceEventViewModel>>("/api/events");

        var userId = HttpContext.Session.GetString("UserId");

        return (events ?? new List<RaceEventViewModel>())
            .Where(e => e.OrganiserId.ToString() == userId)
            .OrderBy(e => e.Date)
            .ToList();
    }

    // ORGANISER DASHBOARD

    public async Task<IActionResult> Dashboard()
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await MyEvents();

            var dashboard = new OrganiserDashboardViewModel();

            var today = DateTime.Today;

            foreach (var raceEvent in events)
            {
                var enrolments =
                    await _api.GetAsync<List<EnrolmentViewModel>>(
                        $"/api/enrolments/event/{raceEvent.Id}");

                var count = enrolments?.Count ?? 0;

                dashboard.Events.Add(
                    new OrganiserEventSummaryViewModel
                    {
                        Event = raceEvent,
                        TotalEnrolments = count
                    });
            }

            dashboard.TotalEvents = dashboard.Events.Count;

            dashboard.UpcomingEvents = dashboard.Events.Count(
                x => x.Event.Date.Date >= today);

            dashboard.PastEvents = dashboard.Events.Count(
                x => x.Event.Date.Date < today);

            dashboard.TotalEnrolments = dashboard.Events.Sum(
                x => x.TotalEnrolments);

            return View(dashboard);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(new OrganiserDashboardViewModel());
        }
    }

    public async Task<IActionResult> Events()
    {
        var access = CheckAccess();
        if (access != null) return access;

        try
        {
            return View(await MyEvents());
        }
        catch
        {
            TempData["Error"] = "Unable to load events.";
            return View(new List<RaceEventViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        var access = CheckAccess();
        if (access != null) return access;

        return View(new EventFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        var access = CheckAccess();
        if (access != null) return access;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new
            {
                model.Name,
                model.Description,
                model.Date,
                model.Location,
                model.DistanceKm,
                model.EventType
            };

            await _api.PostAsync<object, RaceEventViewModel>(
                "/api/events", request);

            TempData["Success"] = "Event created successfully.";

            return RedirectToAction("Events");
        }
        catch
        {
            ModelState.AddModelError(
                "", "Could not create event. Check the information and try again.");

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var access = CheckAccess();
        if (access != null) return access;

        try
        {
            var item = await _api.GetAsync<RaceEventViewModel>(
                $"/api/events/{id}");

            if (item == null ||
                item.OrganiserId.ToString() !=
                HttpContext.Session.GetString("UserId"))
                return NotFound();

            var model = new EventFormViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Date = item.Date,
                Location = item.Location,
                DistanceKm = item.DistanceKm,
                EventType = item.EventType
            };

            return View("Create", model);
        }
        catch
        {
            TempData["Error"] = "Unable to load event.";
            return RedirectToAction("Events");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id, EventFormViewModel model)
    {
        var access = CheckAccess();
        if (access != null) return access;

        if (!ModelState.IsValid)
            return View("Create", model);

        try
        {
            var request = new
            {
                model.Name,
                model.Description,
                model.Date,
                model.Location,
                model.DistanceKm,
                model.EventType
            };

            await _api.PutAsync<object, RaceEventViewModel>(
                $"/api/events/{id}", request);

            TempData["Success"] = "Event updated successfully.";

            return RedirectToAction("Events");
        }
        catch
        {
            ModelState.AddModelError(
                "", "Could not update event.");

            return View("Create", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var access = CheckAccess();
        if (access != null) return access;

        try
        {
            await _api.DeleteAsync($"/api/events/{id}");

            TempData["Success"] = "Event deleted successfully.";
        }
        catch
        {
            TempData["Error"] =
                "Unable to delete event. Please try again.";
        }

        return RedirectToAction("Events");
    }

    // CATEGORY MANAGEMENT

    public async Task<IActionResult> Categories(Guid eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            var raceEvent = await _api.GetAsync<RaceEventViewModel>(
                $"/api/events/{eventId}");

            if (raceEvent == null)
                return NotFound();

            if (raceEvent.OrganiserId.ToString() !=
                HttpContext.Session.GetString("UserId"))
                return Forbid();

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"/api/events/{eventId}/categories");

            ViewBag.EventName = raceEvent.Name;
            ViewBag.EventId = eventId;

            return View(categories ?? new List<CategoryViewModel>());
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Events");
        }
    }

    [HttpGet]
    public IActionResult CreateCategory(Guid eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        return View(new CategoryFormViewModel
        {
            EventId = eventId,
            CategoryType = "Age"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CategoryFormViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new
            {
                Name = model.Name,
                CategoryType = model.CategoryType
            };

            await _api.PostAsync<object, CategoryViewModel>(
                $"/api/events/{model.EventId}/categories", request);

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction("Categories",
                new { eventId = model.EventId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(Guid eventId, Guid categoryId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"/api/events/{eventId}/categories");

            var category = categories?.FirstOrDefault(x => x.Id == categoryId);

            if (category == null)
                return NotFound();

            return View(new CategoryFormViewModel
            {
                Id = category.Id,
                EventId = eventId,
                Name = category.Name,
                CategoryType = category.CategoryType
            });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Categories", new { eventId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(CategoryFormViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new
            {
                Name = model.Name,
                CategoryType = model.CategoryType
            };

            await _api.PutAsync<object, CategoryViewModel>(
                $"/api/events/{model.EventId}/categories/{model.Id}",
                request);

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Categories",
                new { eventId = model.EventId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(Guid eventId, Guid categoryId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            await _api.DeleteAsync(
                $"/api/events/{eventId}/categories/{categoryId}");

            TempData["Success"] = "Category deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                "The category could not be deleted. It may already have enrolments. " +
                ex.Message;
        }

        return RedirectToAction("Categories", new { eventId });
    }

    // VIEW PARTICIPANT ENROLMENTS FOR AN EVENT

    public async Task<IActionResult> Enrolments(Guid eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            var raceEvent = await _api.GetAsync<RaceEventViewModel>(
                $"/api/events/{eventId}");

            if (raceEvent == null)
                return NotFound();

            if (raceEvent.OrganiserId.ToString() !=
                HttpContext.Session.GetString("UserId"))
                return Forbid();

            var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>(
                $"/api/enrolments/event/{eventId}");

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"/api/events/{eventId}/categories");

            ViewBag.EventName = raceEvent.Name;
            ViewBag.EventId = eventId;
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Events");
        }
    }

    // RESULTS MANAGEMENT

    public async Task<IActionResult> Results(Guid eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        try
        {
            var raceEvent = await _api.GetAsync<RaceEventViewModel>(
                $"/api/events/{eventId}");

            if (raceEvent == null)
                return NotFound();

            if (raceEvent.OrganiserId.ToString() !=
                HttpContext.Session.GetString("UserId"))
                return Forbid();

            var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>(
                $"/api/enrolments/event/{eventId}");

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"/api/events/{eventId}/categories");

            var entries = (enrolments ?? new List<EnrolmentViewModel>())
                .Select(enrolment => new ResultEntryViewModel
                {
                    EventId = eventId,
                    EnrolmentId = enrolment.Id,
                    ParticipantId = enrolment.ParticipantId,
                    CategoryName = categories?
                        .FirstOrDefault(c => c.Id == enrolment.CategoryId)?.Name
                        ?? "Category unavailable"
                })
                .ToList();

            ViewBag.EventName = raceEvent.Name;
            ViewBag.EventId = eventId;

            return View(entries);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Events");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveResult(ResultEntryViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Enter a valid finishing time in HH:MM:SS format and a position greater than zero.";

            return RedirectToAction("Results", new { eventId = model.EventId });
        }

        if (!TimeSpan.TryParse(
            model.FinishTime,
            System.Globalization.CultureInfo.InvariantCulture,
            out var finishTime) || finishTime <= TimeSpan.Zero)
        {
            TempData["Error"] = "The finishing time is invalid.";
            return RedirectToAction("Results", new { eventId = model.EventId });
        }

        try
        {
            var request = new
            {
                EnrolmentId = model.EnrolmentId,
                FinishTime = finishTime,
                Position = model.Position
            };

            await _api.PostAsync<object, RaceResultViewModel>(
                "/api/results", request);

            TempData["Success"] = "Participant result saved successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Could not save the result: " + ex.Message;
        }

        return RedirectToAction("Results", new { eventId = model.EventId });
    }
}
