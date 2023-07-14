using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Doeminos
{
    public string? Id { get; set; }

    public string? Kunde { get; set; }

    public string? Straße { get; set; }

    public string? Hausnummer { get; set; }

    public string? Plz { get; set; }

    public string? Ort { get; set; }
}
