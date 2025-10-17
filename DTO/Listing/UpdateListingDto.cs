
namespace PetShop.DTO.Listing
{
    public class UpdateListingDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int TypeId { get; set; }
        public int CurrencyId { get; set; }
        public int? PhotoId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
