using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class ListingType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
