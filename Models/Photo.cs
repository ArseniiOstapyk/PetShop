using System;
using System.Collections.Generic;

namespace PetShop.Models;

public partial class Photo
{
    public int Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
