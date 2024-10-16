using Homies.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Homies.Constants.ValidationConstants;

namespace Homies.Data
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EventNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionNameMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        public string OrganiserId { get; set; } = null!;

        [Required]
        public IdentityUser Organiser { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public int TypeId { get; set; }

        [ForeignKey(nameof(TypeId))]

        [Required]
        //'Type' is an ambiguous reference between 'Homies.Data.Type' and 'System.Type'
        public Data.Type Type { get; set; } = null!;

        public List<EventParticipant> EventsParticipants { get; set; } = new List<EventParticipant>();

        public bool IsDeleted { get; set; }
    }
}
