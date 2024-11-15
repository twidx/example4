using System;
using System.Collections.Generic;
using Example2.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Example2.Database.Context;

public partial class WebDbContext : DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Account { get; set; }

    public virtual DbSet<TableA> TableA { get; set; }

    public virtual DbSet<TableB> TableB { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountNo);

            entity.Property(e => e.AccountNo).HasColumnType("TEXT (50)");
            entity.Property(e => e.Name).HasColumnType("TEXT (50)");
            entity.Property(e => e.Password).HasColumnType("TEXT (50)");
        });

        modelBuilder.Entity<TableA>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.Property(e => e.Code).HasColumnType("TEXT (10)");
            entity.Property(e => e.Name).HasColumnType("TEXT (20)");
        });

        modelBuilder.Entity<TableB>(entity =>
        {
            entity.HasKey(e => e.RowId);

            entity.Property(e => e.Code).HasColumnType("TEXT (10)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
