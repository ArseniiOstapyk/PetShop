using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class User
{
    public int Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual ICollection<OrderListing> OrderListings { get; set; } = new List<OrderListing>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual Role? Role { get; set; }
}
