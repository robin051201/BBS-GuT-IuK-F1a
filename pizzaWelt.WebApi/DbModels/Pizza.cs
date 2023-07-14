using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Pizza
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Preisgroß { get; set; }

    public decimal Preisklein { get; set; }

    public virtual ICollection<Pizzazutaten> Pizzazutaten { get; set; } = new List<Pizzazutaten>();

    public virtual ICollection<Warenkorb> Warenkorb { get; set; } = new List<Warenkorb>();
}
