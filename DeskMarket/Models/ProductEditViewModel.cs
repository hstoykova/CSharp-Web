using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.ValidationConstants;

namespace DeskMarket.Models
{
    public class ProductEditViewModel
    {
        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(ProductPriceStartRange, ProductPriceEndRange)]
        public decimal Price { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public string AddedOn { get; set; } = DateTime.Today.ToString(AddedOnDateFormat);

        [Required]
        public int CategoryId { get; set; }

        public required string SellerId { get; set; }

        public List<Category> Categories { get; set; } = new();
    }
}
