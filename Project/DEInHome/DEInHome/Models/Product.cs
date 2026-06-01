using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace DEInHome.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Article { get; set; } = null!;

    public int ProductTypeId { get; set; }

    public int UnitId { get; set; }

    public int Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufactureId { get; set; }

    public int CategoryId { get; set; }

    public int Discount { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();

    public virtual Manufacture Manufacture { get; set; } = null!;

    public virtual ProductType ProductType { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;

    public double? CaclPrice
    {
        get
        {
            if (Discount <= 0)
                return Price;

            return Price * (100 - Discount) / 100;
        }
    }

    public Bitmap? ProductImage
    {
        get
        {
            string fileName = string.IsNullOrWhiteSpace(Image) ? "picture.png" : Image;

            string imagePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Images", fileName));

            if (!File.Exists(imagePath))
                return null;

            return new Bitmap(imagePath);
        }
    }
}
