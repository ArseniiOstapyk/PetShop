namespace PetShop.DTO.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
    }
}
