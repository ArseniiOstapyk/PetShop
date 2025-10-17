namespace PetShop.DTO.Listing
{
    public class ListingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public string Owner { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        public int OwnerId { get; set; }
    }
}
