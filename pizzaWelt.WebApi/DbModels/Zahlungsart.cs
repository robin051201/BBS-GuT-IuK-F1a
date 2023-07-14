using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Zahlungsart
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Bestellung> Bestellung { get; set; } = new List<Bestellung>();

    public virtual ICollection<Kunde> Kunde { get; set; } = new List<Kunde>();
}
