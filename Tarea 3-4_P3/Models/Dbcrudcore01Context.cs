using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tarea_3_4.Models;

public partial class Dbcrudcore01Context : DbContext
{
    public Dbcrudcore01Context() { }

    public Dbcrudcore01Context(DbContextOptions<Dbcrudcore01Context> options)
        : base(options) { }

    // Tablas de la Base de Datos
    public virtual DbSet<Empleado> Empleados { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<Factura> Facturas { get; set; }
    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Configuración original de tu tabla Empleado
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Empleado__3213E81F6A7C5EAF");
            entity.ToTable("Empleado");
            entity.Property(e => e.ID).HasColumnName("iD");
            entity.Property(e => e.Apellido).HasMaxLength(50).IsUnicode(false).HasColumnName("apellido");
            entity.Property(e => e.Direccion).HasMaxLength(100).IsUnicode(false).HasColumnName("direccion");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).HasColumnName("email");
            entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Numero).HasMaxLength(10).IsUnicode(false).HasColumnName("numero");
        });

        // 2. Nuevas Configuraciones del Inventario y Facturación
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(255).IsUnicode(false);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.Property(e => e.CodigoBarras).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).IsRequired();
            entity.Property(e => e.Costo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Categoria)
                .WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.Property(e => e.RncCedula).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.Telefono).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.Direccion).HasMaxLength(200).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.ToTable("Factura");
            entity.Property(e => e.NumeroFactura).HasMaxLength(20).IsUnicode(false).IsRequired();
            entity.HasIndex(e => e.NumeroFactura).IsUnique();
            entity.Property(e => e.NCF).HasMaxLength(19).IsUnicode(false);
            entity.Property(e => e.TipoNCF).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MetodoPago).HasMaxLength(50).IsUnicode(false).IsRequired();

            entity.HasOne(d => d.Cliente)
                .WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ClienteId);

            entity.HasOne(d => d.Empleado)
                .WithMany() // Vinculado a la tabla Empleado que ya tienes
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.ToTable("DetalleFactura");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Factura)
                .WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra la factura, se borran sus detalles

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}