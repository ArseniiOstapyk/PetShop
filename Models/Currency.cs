using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class Currency
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Sign { get; set; } = null!;

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
