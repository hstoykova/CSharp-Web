using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class ProductInfoViewModel
    {
        public int Id { get; set; }

        public required string ProductName { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSeller { get; set; }

        public bool HasBought { get; set; }
    }
}
