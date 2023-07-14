using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using pizzaWelt.WebApi.DbModels;

namespace pizzaWelt.WebApi.Context;

public partial class pizzaContext : DbContext
{
    public pizzaContext()
    {
    }

    public pizzaContext(DbContextOptions<pizzaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adresse> Adresse { get; set; }

    public virtual DbSet<Bestellung> Bestellung { get; set; }

    public virtual DbSet<Kunde> Kunde { get; set; }

    public virtual DbSet<Lieferart> Lieferart { get; set; }

    public virtual DbSet<Mitarbeiter> Mitarbeiter { get; set; }

    public virtual DbSet<Pizza> Pizza { get; set; }

    public virtual DbSet<Pizzazutaten> Pizzazutaten { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<Warenkorb> Warenkorb { get; set; }

    public virtual DbSet<Zahlungsart> Zahlungsart { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Data Source=212.227.215.121;Database=pizzeria; Uid=team_db;Pwd=9n3c&2D2");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adresse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("adresse");

            entity.HasIndex(e => e.Kunde, "adresse_fk0");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Hausnummer).HasColumnName("hausnummer");
            entity.Property(e => e.Kunde).HasColumnName("kunde");
            entity.Property(e => e.Ort)
                .HasColumnType("text")
                .HasColumnName("ort");
            entity.Property(e => e.Plz).HasColumnName("plz");
            entity.Property(e => e.Straße)
                .HasColumnType("text")
                .HasColumnName("straße");

            entity.HasOne(d => d.KundeNavigation).WithMany(p => p.Adresse)
                .HasForeignKey(d => d.Kunde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adresse_fk0");
        });

        modelBuilder.Entity<Bestellung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bestellung");

            entity.HasIndex(e => e.Kunde, "bestellung_fk0");

            entity.HasIndex(e => e.Lieferart, "bestellung_fk1");

            entity.HasIndex(e => e.Zahlungsart, "bestellung_fk2");

            entity.HasIndex(e => e.Mitarbeiter, "bestellung_fk3");

            entity.HasIndex(e => e.Adresse, "bestellung_fk4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Bestellzeitpunkt)
                .HasColumnType("datetime")
                .HasColumnName("bestellzeitpunkt");
            entity.Property(e => e.Gesamtpreis)
                .HasPrecision(10)
                .HasColumnName("gesamtpreis");
            entity.Property(e => e.Kunde).HasColumnName("kunde");
            entity.Property(e => e.Lieferart).HasColumnName("lieferart");
            entity.Property(e => e.Mitarbeiter).HasColumnName("mitarbeiter");
            entity.Property(e => e.Zahlungsart).HasColumnName("zahlungsart");

            entity.HasOne(d => d.KundeNavigation).WithMany(p => p.Bestellung)
                .HasForeignKey(d => d.Kunde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bestellung_fk0");

            entity.HasOne(d => d.LieferartNavigation).WithMany(p => p.Bestellung)
                .HasForeignKey(d => d.Lieferart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bestellung_fk1");

            entity.HasOne(d => d.MitarbeiterNavigation).WithMany(p => p.Bestellung)
                .HasForeignKey(d => d.Mitarbeiter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bestellung_fk3");

            entity.HasOne(d => d.ZahlungsartNavigation).WithMany(p => p.Bestellung)
                .HasForeignKey(d => d.Zahlungsart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bestellung_fk2");
        });

        modelBuilder.Entity<Kunde>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("kunde");

            entity.HasIndex(e => e.Zahlungsart, "kunde_fk0");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nachname)
                .HasColumnType("text")
                .HasColumnName("nachname");
            entity.Property(e => e.Telefonnummer).HasColumnName("telefonnummer");
            entity.Property(e => e.Vorname)
                .HasColumnType("text")
                .HasColumnName("vorname");
            entity.Property(e => e.Zahlungsart).HasColumnName("zahlungsart");

            entity.HasOne(d => d.ZahlungsartNavigation).WithMany(p => p.Kunde)
                .HasForeignKey(d => d.Zahlungsart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kunde_fk0");
        });

        modelBuilder.Entity<Lieferart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lieferart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.Preis)
                .HasPrecision(10)
                .HasColumnName("preis");
        });

        modelBuilder.Entity<Mitarbeiter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mitarbeiter");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nachname)
                .HasColumnType("text")
                .HasColumnName("nachname");
            entity.Property(e => e.Vorname)
                .HasColumnType("text")
                .HasColumnName("vorname");
        });

        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pizza");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.Preisgroß)
                .HasPrecision(10)
                .HasColumnName("preisgroß");
            entity.Property(e => e.Preisklein)
                .HasPrecision(10)
                .HasColumnName("preisklein");
        });

        modelBuilder.Entity<Pizzazutaten>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pizzazutaten");

            entity.HasIndex(e => e.Pizza, "pizzazutaten_fk0");

            entity.HasIndex(e => e.Zutaten, "pizzazutaten_fk1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Pizza).HasColumnName("pizza");
            entity.Property(e => e.Zutaten).HasColumnName("zutaten");

            entity.HasOne(d => d.PizzaNavigation).WithMany(p => p.Pizzazutaten)
                .HasForeignKey(d => d.Pizza)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pizzazutaten_fk0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Kunde, "user_fk0");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasColumnType("text")
                .HasColumnName("email");
            entity.Property(e => e.Kunde).HasColumnName("kunde");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.Passwort)
                .HasColumnType("text")
                .HasColumnName("passwort");

            entity.HasOne(d => d.KundeNavigation).WithMany(p => p.User)
                .HasForeignKey(d => d.Kunde)
                .HasConstraintName("user_fk0");
        });

        modelBuilder.Entity<Warenkorb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("warenkorb");

            entity.HasIndex(e => e.Pizza, "warenkorb_fk0");

            entity.HasIndex(e => e.Bestellung, "warenkorb_fk1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bestellung).HasColumnName("bestellung");
            entity.Property(e => e.Groesse)
                .HasColumnType("enum('klein','gross')")
                .HasColumnName("groesse");
            entity.Property(e => e.Pizza).HasColumnName("pizza");

            entity.HasOne(d => d.BestellungNavigation).WithMany(p => p.Warenkorb)
                .HasForeignKey(d => d.Bestellung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("warenkorb_fk1");

            entity.HasOne(d => d.PizzaNavigation).WithMany(p => p.Warenkorb)
                .HasForeignKey(d => d.Pizza)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("warenkorb_fk0");
        });

        modelBuilder.Entity<Zahlungsart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("zahlungsart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
