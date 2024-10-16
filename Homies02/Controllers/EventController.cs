using Homies.Data;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Globalization;
using System.Security.Claims;
using static Homies.Constants.ValidationConstants;

namespace Homies.Controllers
{
    public class EventController : Controller
    {
        //For logged users only
        //[Authorize]

        private readonly HomiesDbContext context;
        public EventController(HomiesDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await context.Events
                .Where(e => e.IsDeleted == false)
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(ValidationDateFormat),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName ?? string.Empty
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddEventViewModel();
            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                model.Types = await GetTypes();

                return View(model);
            }

            DateTime startDate;
            DateTime endDate;

            if (DateTime.TryParseExact(model.Start, ValidationDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out startDate) == false)
            {
                ModelState.AddModelError(nameof(model.Start), "Invalid date format");
                model.Types = await GetTypes();

                return View(model);
            }

            if (DateTime.TryParseExact(model.End, ValidationDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out endDate) == false)
            {
                ModelState.AddModelError(nameof(model.End), "Invalid date format");
                model.Types = await GetTypes();

                return View(model);
            }

            if (startDate > endDate)
            {
                ModelState.AddModelError(nameof(model.Start), "Start date is after end date");
                model.Types = await GetTypes();

                return View(model);
            }

            Event addEvent = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                OrganiserId = GetCurrentUserId() ?? string.Empty,
                Start = startDate,
                End = endDate,
                TypeId = model.TypeId
            };

            await context.Events.AddAsync(addEvent);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var model = await context.Events
                .Where(e => e.IsDeleted == false)
                .Where(e => e.EventsParticipants.Any(ep => ep.HelperId == GetCurrentUserId()))
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(ValidationDateFormat),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName ?? string.Empty
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Data.Type>> GetTypes()
    {
        return await context.Types.ToListAsync();
    }
}
}
