using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.Common.Models;

public partial class CommonDBContext : DbContext
{
    public CommonDBContext()
    {
    }

    public CommonDBContext(DbContextOptions<CommonDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Unit");

            entity.HasIndex(e => e.UnitName, "UK_Unit_UnitName").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.ChargeCode)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OriginSystem)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UnitCode)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.UnitDefinitionIdUnitQty).HasColumnName("UnitDefinitionId_UnitQty");
            entity.Property(e => e.UnitDefinitionIdUnitValue).HasColumnName("UnitDefinitionId_UnitValue");
            entity.Property(e => e.UnitDescription)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.UnitHeaderQuery)
                .HasMaxLength(4096)
                .IsUnicode(false);
            entity.Property(e => e.UnitName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrepQuery)
                .HasMaxLength(4096)
                .IsUnicode(false);
            entity.Property(e => e.UnitUpdateQuery)
                .HasMaxLength(1024)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ValueTypeName)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
