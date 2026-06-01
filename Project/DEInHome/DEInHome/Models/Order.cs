using System;
using System.Collections.Generic;

namespace DEInHome.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int PickupAdressId { get; set; }

    public int UserId { get; set; }

    public int ReceiptCode { get; set; }

    public int OrderStatusId { get; set; }

    public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual PickupAdress PickupAdress { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
