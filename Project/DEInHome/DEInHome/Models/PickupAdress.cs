using System;
using System.Collections.Generic;

namespace DEInHome.Models;

public partial class PickupAdress
{
    public int Id { get; set; }

    public int Code { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
