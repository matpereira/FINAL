﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using TrabajoFinalRestaurante.Domain;
using TrabajoFinalRestaurante.Domain.Entities;
using TrabajoFinalRestaurante.Entities;

namespace TrabajoFinalRestaurante.Repository;

public partial class ReservaRestaurantContext : DbContext
{
    public ReservaRestaurantContext(DbContextOptions<ReservaRestaurantContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RangoReserva> RangoReservas { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RangoReserva>(entity =>
        {
            entity.HasKey(e => e.IdRangoReserva).HasName("PK__rango_re__E110E7BB98AD945B");

            entity.ToTable("rango_reserva");

            entity.Property(e => e.IdRangoReserva).HasColumnName("id_rango_reserva");
            entity.Property(e => e.Cupo).HasColumnName("cupo");
            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.CodReserva).HasName("PK__reserva__12EE268693EAEC0E");

            entity.ToTable("reserva");

            entity.Property(e => e.CodReserva)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cod_reserva");
            entity.Property(e => e.ApellidoPersona)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido_persona");
            entity.Property(e => e.CantidadPersonas).HasColumnName("cantidad_personas");
            entity.Property(e => e.Celular)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("celular");
            entity.Property(e => e.Dni)
                .IsRequired()
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("dni");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaReserva)
                .HasColumnType("datetime")
                .HasColumnName("fecha_reserva");
            entity.Property(e => e.IdRangoReserva).HasColumnName("id_rango_reserva");
            entity.Property(e => e.Mail)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mail");
            entity.Property(e => e.NombrePersona)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_persona");

            entity.HasOne(d => d.IdRangoReservaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdRangoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reserva__id_rang__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}