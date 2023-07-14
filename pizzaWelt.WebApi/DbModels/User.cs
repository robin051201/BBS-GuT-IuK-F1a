using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Passwort { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Kunde { get; set; }

    public virtual Kunde? KundeNavigation { get; set; }
}
