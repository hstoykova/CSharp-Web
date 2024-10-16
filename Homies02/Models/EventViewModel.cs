using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Homies.Constants.ValidationConstants;

namespace Homies.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Start { get; set; } = DateTime.Today.ToString(ValidationDateFormat);

        public string Type { get; set; } = null!;

        public string Organiser { get; set; } = null!;
    }
}
