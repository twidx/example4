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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountNo);

            entity.Property(e => e.AccountNo).HasColumnType("TEXT (50)");
            entity.Property(e => e.Name).HasColumnType("TEXT (50)");
            entity.Property(e => e.Password).HasColumnType("TEXT (50)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
