using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class OrderListing
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ListingId { get; set; }

    public int? OwnerId { get; set; }

    public virtual Listing Listing { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User? Owner { get; set; }
}
