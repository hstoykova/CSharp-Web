using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.ValidationConstants;

namespace DeskMarket.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public List<Product> Products { get; set; } = new();
    }
}
