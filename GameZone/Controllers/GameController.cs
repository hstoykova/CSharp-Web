using GameZone.Data;
using GameZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using static GameZone.Shared.ValidationConstants;

namespace GameZone.Controllers
{
    //For logged users only

    [Authorize]

    public class GameController : Controller
    {
        private readonly GameZoneDbContext context;
        public GameController(GameZoneDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await context.Games
                .Where(g => g.IsDeleted == false)
                .Select(g => new GameInfoViewModel
                {
                    Id = g.Id,
                    Genre = g.Genre.Name,
                    ImageUrl = g.ImageUrl,
                    Publisher = g.Publisher.UserName ?? string.Empty,
                    ReleasedOn = g.ReleasedOn.ToString(ReleasedOnDateFormat),
                    Title = g.Title
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new GameViewModel();
            model.Genres = await GetGenres();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GameViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                model.Genres = await GetGenres();

                return View(model);
            }

            DateTime releasedOn;

            if (DateTime.TryParseExact(model.ReleasedOn, ReleasedOnDateFormat, CultureInfo.CurrentCulture,DateTimeStyles.None , out releasedOn) == false)
            {
                ModelState.AddModelError(nameof(model.ReleasedOn), "Invalid date format");
                model.Genres = await GetGenres();

                return View(model);
            }

            Game game = new Game()
            {
                Description = model.Description,
                GenreId = model.GenreId,
                ImageUrl = model.ImageUrl,
                PublisherId = GetCurrentUserId() ?? string.Empty,
                ReleasedOn = releasedOn,
                Title = model.Title
            };

            await context.Games.AddAsync(game);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new GameViewModel();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GameViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyZone()
        {
            return View(new List<GameInfoViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> AddToMyZone(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StrikeOut(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        }

        private async Task<List<Genre>> GetGenres()
        {
            return await context.Genres.ToListAsync();
        }
    }
}
