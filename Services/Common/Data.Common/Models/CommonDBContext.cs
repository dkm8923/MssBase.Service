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

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<UnitDefinition> UnitDefinitions { get; set; }

    public virtual DbSet<UnitGroupColumn> UnitGroupColumns { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Commission>(entity =>
        {
            entity.ToTable("Commission");

            entity.HasIndex(e => e.RegionalServiceProvider, "IDX_Commission_RegionalServiceProvider");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CompletedRouteCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CyclePackageCount).HasDefaultValue(0);
            entity.Property(e => e.DroppedRouteCount).HasDefaultValue(0);
            entity.Property(e => e.ExpectedRouteCount).HasDefaultValue(0);
            entity.Property(e => e.Facility)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.MileageCount).HasDefaultValue(0);
            entity.Property(e => e.PackageIncentiveTier)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.QuickCoverageCount).HasDefaultValue(0);
            entity.Property(e => e.RegionalServiceProvider)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.VehicleCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.ToTable("Rate");

            entity.HasIndex(e => new { e.Active, e.UnitId, e.UnitValue, e.RateAmt, e.StartDate, e.EndDate, e.Facility }, "UK_Rate").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Facility)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.MaxLimit).HasDefaultValue(99999);
            entity.Property(e => e.MinLimit).HasDefaultValue(1);
            entity.Property(e => e.RateAmt).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.UnitValue)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
        });

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

        modelBuilder.Entity<UnitDefinition>(entity =>
        {
            entity.ToTable("UnitDefinition");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.ConditionalAdjustmentColumn)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DestinationColumn)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.EvaluateAsString).HasDefaultValue(true);
            entity.Property(e => e.GroupBy).HasDefaultValue(true);
            entity.Property(e => e.GroupByColumn)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.ListObjectName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.OriginSystem)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.PkgQtyColumn)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.SourceColumn)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.SqlDataType)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.SupplementalGroupByColumn)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UnitQtyColumn)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.UnitQuery).HasDefaultValue(true);
            entity.Property(e => e.UnitQueryColumn)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.UnitValueColumn)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UseList).HasDefaultValue(true);
            entity.Property(e => e.UserFriendlyDescription)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UnitGroupColumn>(entity =>
        {
            entity.ToTable("UnitGroupColumn", tb =>
                {
                    tb.HasTrigger("FixSortOrder_UnitGroupColumn");
                    tb.HasTrigger("LogChange_UnitGroupColumn");
                    tb.HasTrigger("LogDelete_UnitGroupColumn");
                });

            entity.HasIndex(e => new { e.UnitId, e.UnitDefinitionId }, "UK_UnitGroupColumn").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0)
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Unit).WithMany(p => p.UnitGroupColumns)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Unit2UnitGroupColumn");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
