using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Zutaten
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double Preis { get; set; }

    public virtual ICollection<Pizzazutaten> Pizzazutaten { get; set; } = new List<Pizzazutaten>();
}
