using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GameZone.Shared.ValidationConstants;

namespace GameZone.Data
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(TitleMinLenght)]
        [MaxLength(TitleMaxLenght)]
        public string Title { get; set; } = null!;

        [Required]
        //[MinLength(DescriptionMinLenght)]
        [MaxLength(DescriptionMaxLenght)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        public string PublisherId { get; set; } = null!;

        [Required]
        public IdentityUser Publisher { get; set; } = null!;

        [Required]
        public DateTime ReleasedOn { get; set; }

        [Required]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]

        //[Required]
        public Genre Genre { get; set; } = null!;

        public List<GamerGame> GamersGames { get; set; } = new List<GamerGame>();

        public bool IsDeleted { get; set; }
    }
}
