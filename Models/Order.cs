using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int OwnerId { get; set; }

    public bool? IsPaid { get; set; }

    public virtual ICollection<OrderListing> OrderListings { get; set; } = new List<OrderListing>();

    public virtual User Owner { get; set; } = null!;
}
