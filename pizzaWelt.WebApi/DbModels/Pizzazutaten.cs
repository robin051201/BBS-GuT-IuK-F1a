using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Pizzazutaten
{
    public int Id { get; set; }

    public int Pizza { get; set; }

    public int Zutaten { get; set; }

    public virtual Pizza PizzaNavigation { get; set; } = null!;
}
