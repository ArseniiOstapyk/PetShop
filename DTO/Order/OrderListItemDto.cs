namespace PetShop.DTO.Order
{
    public class OrderListItemDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int ListingsCount { get; set; }
        public double TotalSum { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
