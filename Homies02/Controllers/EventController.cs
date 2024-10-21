using Homies.Data;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Protocol;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Globalization;
using System.Security.Claims;
using static Homies.Constants.ValidationConstants;

namespace Homies.Controllers
{
    //For logged users only
    [Authorize]

    public class EventController : Controller
    {
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


        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            Event? eventToJoin = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            if (eventToJoin == null || eventToJoin.IsDeleted)
            {
                throw new ArgumentException("Invalid id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (eventToJoin.EventsParticipants.Any(ep => ep.HelperId == currentUserId))
            {
                return RedirectToAction(nameof(All));
            }

            EventParticipant participant = new()
            {
                EventId = eventToJoin.Id,
                HelperId = currentUserId
            };

            await context.EventsParticipants.AddAsync(participant);

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Joined)); 
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            Event? eventToJoin = await context.Events
                .Where(e => e.Id == id)
                .AsNoTracking()
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            if (eventToJoin == null || eventToJoin.IsDeleted)
            {
                throw new ArgumentException("Invalid id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (eventToJoin.EventsParticipants.Any(ep => ep.HelperId == currentUserId))
            {
                EventParticipant participant = new()
                {
                    EventId = eventToJoin.Id,
                    HelperId = currentUserId
                };

                context.EventsParticipants.Remove(participant);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(All));
            }

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await context.Events
                .Where(e => e.Id == id)
                .Where(e => e.IsDeleted == false)
                .Where(e => e.OrganiserId == GetCurrentUserId())
                .AsNoTracking()
                .Select(e => new AddEventViewModel()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(ValidationDateFormat),
                    End = e.End.ToString(ValidationDateFormat),
                    TypeId = e.TypeId
                })
                .FirstOrDefaultAsync();

            if (model is null)
            {
                return NotFound();
            }

            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddEventViewModel model, int id)
        {
            if (ModelState.IsValid == false)
            {
                model.Types = await GetTypes();

                return View(model);
            }

            DateTime startDate;
            DateTime endDate;

            if (DateTime.TryParseExact(model.Start, ValidationDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) == false)
            {
                ModelState.AddModelError(nameof(model.Start), "Invalid date format");
                model.Types = await GetTypes();

                return View(model);
            }

            if (DateTime.TryParseExact(model.End, ValidationDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate) == false)
            {
                ModelState.AddModelError(nameof(model.End), "Invalid date format");
                model.Types = await GetTypes();

                return View(model);
            }

            Event? editEvent = await context.Events.FindAsync(id);

            if (editEvent == null || editEvent.IsDeleted)
            {
                throw new ArgumentException("Invalid id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (editEvent.OrganiserId != currentUserId)
            {
                return RedirectToAction(nameof(All));
            }

            editEvent.Name = model.Name;
            editEvent.Description = model.Description;
            editEvent.Start = startDate;
            editEvent.End = endDate;
            editEvent.TypeId = model.TypeId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details (int id)
        {
            var model = await context.Events
                .Where(e => e.IsDeleted == false)
                .Where(e => e.Id == id)
                .AsNoTracking()
                .Select(e => new EventDetailsViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(ValidationDateFormat),
                    End = e.End.ToString(ValidationDateFormat),
                    Organiser = e.Organiser.UserName ?? string.Empty,
                    CreatedOn = e.CreatedOn.ToString(ValidationDateFormat),
                    Type = e.Type.Name,
                })
                .FirstOrDefaultAsync();

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

