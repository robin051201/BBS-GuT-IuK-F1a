using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Lieferart
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Preis { get; set; }

    public virtual ICollection<Bestellung> Bestellung { get; set; } = new List<Bestellung>();
}
