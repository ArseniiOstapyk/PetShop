using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
