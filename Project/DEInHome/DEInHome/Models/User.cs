using System;
using System.Collections.Generic;

namespace DEInHome.Models;

public partial class User
{
    public int Id { get; set; }

    public int UserRoleId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual UserRole UserRole { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName} {Patronymic}";
}
