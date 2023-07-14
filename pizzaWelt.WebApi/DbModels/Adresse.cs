using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Adresse
{
    public int Id { get; set; }

    public int Kunde { get; set; }

    public string Straße { get; set; } = null!;

    public int Hausnummer { get; set; }

    public int Plz { get; set; }

    public string Ort { get; set; } = null!;

    public virtual Kunde KundeNavigation { get; set; } = null!;
}
