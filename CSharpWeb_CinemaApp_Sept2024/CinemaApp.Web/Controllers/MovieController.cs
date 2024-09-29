﻿using CinemaApp.Data;
using CinemaApp.Data.Models;
using CinemaApp.Web.ViewModels.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static CinemaApp.Common.EntityValidationConstants.Movie;

namespace CinemaApp.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly CinemaDbContext dbContext;

        public MovieController(CinemaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Movie> allMovies = this.dbContext.Movies.ToList();

            return this.View(allMovies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(AddMovieInputModel inputModel)
        {

            // TODO: Add form model + validation

            bool isReleaseDateValid = DateTime.TryParseExact(inputModel.ReleaseDate, ReleaseDateFormat, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate);


            if (!isReleaseDateValid)
            {
                // ModelState becomes invalid => IsValid = false;
                this.ModelState.AddModelError(nameof(inputModel.ReleaseDate), String.Format("The Release Date must be in the following format: {0}", ReleaseDateFormat));

                //return this.View(inputModel);
            }

            if (!this.ModelState.IsValid)
            {
                // Render the same form with user entered values + model errors
                return this.View(inputModel);
            }

            Movie movie = new Movie()
            {
                Title = inputModel.Title,
                Genre = inputModel.Genre,
                ReleaseDate = releaseDate,
                Director = inputModel.Director,
                Duration = inputModel.Duration,
                Description = inputModel.Description
            };

            this.dbContext.Movies.Add(movie);
            this.dbContext.SaveChanges();

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            bool isIdValid = Guid.TryParse(id, out Guid guidId);

            if (!isIdValid)
            {
                //Invalid id format
                return this.RedirectToAction(nameof(Index));
            }

            Movie? movie = this.dbContext.
                Movies.
                FirstOrDefault(m => m.Id == guidId);

            if (movie == null)
            {
                // Non-existing movie guid
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(movie);
        }
    }
}
