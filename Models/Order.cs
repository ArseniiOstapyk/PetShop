using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int OwnerId { get; set; }

    public int ListingId { get; set; }

    public bool? IsPaid { get; set; }

    public virtual Listing Listing { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;
}
