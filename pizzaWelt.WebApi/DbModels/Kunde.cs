using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Kunde
{
    public int Id { get; set; }

    public string Vorname { get; set; } = null!;

    public string Nachname { get; set; } = null!;

    public int Telefonnummer { get; set; }

    public int Zahlungsart { get; set; }

    public virtual ICollection<Adresse> Adresse { get; set; } = new List<Adresse>();

    public virtual ICollection<Bestellung> Bestellung { get; set; } = new List<Bestellung>();

    public virtual ICollection<User> User { get; set; } = new List<User>();

    public virtual Zahlungsart ZahlungsartNavigation { get; set; } = null!;
}
