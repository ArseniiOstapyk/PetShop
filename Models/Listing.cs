using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class Listing
{
    public int Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public double Price { get; set; }

    public int TypeId { get; set; }

    public int CurrencyId { get; set; }

    public int? PhotoId { get; set; }

    public bool IsAvailable { get; set; }

    public int OwnerId { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User Owner { get; set; } = null!;

    public virtual Photo? Photo { get; set; }

    public virtual ListingType Type { get; set; } = null!;
}
