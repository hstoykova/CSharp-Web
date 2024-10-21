namespace DeskMarket.Constants
{
    public static class ValidationConstants
    {
        public const int ProductNameMinLength = 2;
        public const int ProductNameMaxLength = 60;

        public const int DescriptionMinLength = 10;
        public const int DescriptionMaxLength = 250;

        public const string AddedOnDateFormat = "dd-MM-yyyy";

        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 20;

        public const double ProductPriceStartRange = 1.00d;
        public const double ProductPriceEndRange = 3000.00d;
    }
}
