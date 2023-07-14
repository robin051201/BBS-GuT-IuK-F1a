using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Mitarbeiter
{
    public int Id { get; set; }

    public string Vorname { get; set; } = null!;

    public string Nachname { get; set; } = null!;

    public virtual ICollection<Bestellung> Bestellung { get; set; } = new List<Bestellung>();
}
