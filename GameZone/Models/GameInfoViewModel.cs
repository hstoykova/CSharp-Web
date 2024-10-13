﻿namespace GameZone.Models
{
    public class GameInfoViewModel
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public required string ReleasedOn { get; set; }
        public required string Publisher { get; set; }
    }
}