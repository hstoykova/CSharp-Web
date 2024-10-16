using Homies.Models;
using System.ComponentModel.DataAnnotations;
using static Homies.Constants.ValidationConstants;

namespace Homies.Data
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TypeNameMaxLength)]
        public string Name { get; set; } = null!;

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
