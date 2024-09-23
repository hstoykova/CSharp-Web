using CinemaApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CinemaApp.Common.EntityValidationConstants.Movie;

namespace CinemaApp.Data.Configuration
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            // Fluent API
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder.Property(m => m.Genre)
                .IsRequired()
                .HasMaxLength(GenreMaxLength);

            builder.Property(m => m.Director)
                .IsRequired()
                .HasMaxLength(DirectorNameMaxLength);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.HasData(this.SeedMovies());
        }

        private List<Movie> SeedMovies()
        {
            List<Movie> movies = new List<Movie>()
            {
                new Movie
                {
                    Title = "Harry Poter",
                    Genre = "Fantasy",
                    ReleaseDate = new DateTime(2005, 11, 01),
                    Director = "Mike Newel",
                    Duration = 157,
                    Description = "Harry had a thin face, knobbly knees, black hair and bright-green eyes. He wore round glasses held together with a lot of Sellotape because of all the times Dudley had punched him on the nose. The only thing Harry liked about his own appearance was a very thin scar on his forehead which was shaped like a bolt of lightning."
                },
                new Movie
                {
                    Title = "Lord of the rings",
                    Genre = "Fantasy",
                    ReleaseDate = new DateTime(2001, 05, 01),
                    Director = "Peter Jackson",
                    Duration = 178,
                    Description = "The plot of The Lord of the Rings is about the war of the peoples of the fantasy world Middle-earth against a dark lord known as \"Sauron.\" At the same time they try to destroy a ring which would give Sauron a lot of power if he got it, but the only place to destroy the ring is deep into Sauron's land Mordor."
                }
            };

            return movies;
        }
    }
}
