using System.ComponentModel.DataAnnotations;
using static GameZone.Shared.ValidationConstants;

namespace GameZone.Data
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(GenreNameMinLenght)]
        [MaxLength(GenreNameMaxLenght)]
        public string Name { get; set; } = null!;

        public List<Game> Games { get; set; } = new List<Game>();
    }
}
