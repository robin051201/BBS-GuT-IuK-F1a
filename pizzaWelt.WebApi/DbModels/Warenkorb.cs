using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Warenkorb
{
    public int Id { get; set; }

    public int Pizza { get; set; }

    public string Groesse { get; set; } = null!;

    public int Bestellung { get; set; }

    public virtual Bestellung BestellungNavigation { get; set; } = null!;

    public virtual Pizza PizzaNavigation { get; set; } = null!;
}
