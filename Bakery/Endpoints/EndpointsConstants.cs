namespace Bakery.Endpoints;

public static class EndpointsConstants
{
    public static class Category
    {
        public const string NOT_FOUND_MESSAGE = "Category not found.";
        public const string BAD_REQUEST_MESSAGE = "The provided category data is invalid.";
    }

    public static class Option
    {
        public const string NOT_FOUND_MESSAGE = "Option not found.";
        public const string BAD_REQUEST_MESSAGE = "The provided option data is invalid.";
    }

    public static class Product
    {
        public const string NOT_FOUND_MESSAGE = "Product not found.";
        public const string BAD_REQUEST_MESSAGE = "The provided product data is invalid.";
    }
}