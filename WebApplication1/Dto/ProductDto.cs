namespace WebApplication1.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; } = decimal.Zero;

        public string Color { get; set; } = null!;
    }
}
