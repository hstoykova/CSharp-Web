using System.ComponentModel.DataAnnotations;
using static Homies.Constants.ValidationConstants;

namespace Homies.Models
{
    public class AddEventViewModel
    {

        [Required]
        [MinLength(EventNameMinLength)]
        [MaxLength(EventNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(DescriptionNameMinLength)]
        [MaxLength(DescriptionNameMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        public string Start { get; set; } = DateTime.Today.ToString(ValidationDateFormat);

        [Required]
        public string End { get; set; } = DateTime.Today.ToString(ValidationDateFormat);

        [Required]
        public int TypeId { get; set; }

        public List<Data.Type> Types { get; set; } = new ();
    }
}
