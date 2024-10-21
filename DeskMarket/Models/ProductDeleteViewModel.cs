﻿namespace DeskMarket.Models
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public required string Seller { get; set; }
        public required string SellerId { get; set; }

    }
}
