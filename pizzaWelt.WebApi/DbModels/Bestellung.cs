using System;
using System.Collections.Generic;

namespace pizzaWelt.WebApi.DbModels;

public partial class Bestellung
{
    public int Id { get; set; }

    public int Kunde { get; set; }

    public int Lieferart { get; set; }

    public int Zahlungsart { get; set; }

    public int Mitarbeiter { get; set; }

    public int Adresse { get; set; }

    public DateTime Bestellzeitpunkt { get; set; }

    public decimal Gesamtpreis { get; set; }

    public virtual Kunde KundeNavigation { get; set; } = null!;

    public virtual Lieferart LieferartNavigation { get; set; } = null!;

    public virtual Mitarbeiter MitarbeiterNavigation { get; set; } = null!;

    public virtual ICollection<Warenkorb> Warenkorb { get; set; } = new List<Warenkorb>();

    public virtual Zahlungsart ZahlungsartNavigation { get; set; } = null!;
}
