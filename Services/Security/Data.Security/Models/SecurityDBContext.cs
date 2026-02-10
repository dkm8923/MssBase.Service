using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.Security.Models;

public partial class SecurityDBContext : DbContext
{
    public SecurityDBContext()
    {
    }

    public SecurityDBContext(DbContextOptions<SecurityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.HasKey(e => e.ApplicationId);

            entity.HasIndex(e => e.Name, "UQ_Application_Name").IsUnique();

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("ApplicationUser");

            entity.HasKey(e => e.ApplicationUserId);

            entity.HasIndex(e => e.ApplicationId);

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.DateOfBirth)
                .HasPrecision(0);
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.FailedPasswordAttemptCount).HasDefaultValue((short)0);
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.LastLoginDate)
                .HasPrecision(0);
            entity.Property(e => e.LastLockoutDate)
                .HasPrecision(0);
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.LastPasswordChangeDate)
                .HasPrecision(0);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);

            entity.HasOne(d => d.Application)
                .WithMany(p => p.ApplicationUsers)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationUser_Application");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");

            entity.HasKey(e => e.PermissionId);

            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.Name, "UQ_Permission_Name").IsUnique();

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);

            entity.HasOne(d => d.Application)
                .WithMany(p => p.Permissions)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Application");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasKey(e => e.RoleId);

            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.Name, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);

            entity.HasOne(d => d.Application)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Role_Application");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermission");

            entity.HasKey(e => e.RolePermissionId);

            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.PermissionId);
            entity.HasIndex(e => e.RoleId);

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);

            entity.HasOne(d => d.Application)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Application");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Permission");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Role");
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("UserPermission");

            entity.HasKey(e => e.UserPermissionId);

            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.ApplicationUserId);
            entity.HasIndex(e => e.PermissionId);

            entity.Property(e => e.Active).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasPrecision(0);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(0);

            entity.HasOne(d => d.Application)
                .WithMany()
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_Application");

            entity.HasOne(d => d.ApplicationUser)
                .WithMany()
                .HasForeignKey(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_ApplicationUser");

            entity.HasOne(d => d.Permission)
                .WithMany()
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_Permission");
        });

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
