namespace PetShop.DTO.Order
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public bool IsPaid { get; set; }
        public DateTime CreatedOn { get; set; }
        public double TotalSum { get; set; }
        public int ListingsCount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int OrderListingId { get; set; }
        public int ListingId { get; set; }
        public string ListingName { get; set; } = null!;
        public double Price { get; set; }
        public string? SellerEmail { get; set; }
    }
}
