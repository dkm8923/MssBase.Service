using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Data.Security.Configuration;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("Application");

        builder.Property(t => t.ApplicationId).IsRequired();
        builder.Property(t => t.CreatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.UpdatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.UpdatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Active).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateTableData(builder); 
    }
        
        public void CreatePrimaryKey(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(e => e.ApplicationId);
        }
        public void CreateUniqueKey(EntityTypeBuilder<Application> builder)
        {
            builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("UQ_Application_Name");
        }

        // public void CreateForeignKeys(EntityTypeBuilder<Address> builder) 
        // {
        //     builder.HasOne<UsaState>(addr => addr.State)
        //         .WithMany(s => s.Addresses)
        //         .HasForeignKey(addr => addr.StateId);

        //     builder.HasOne<Country>(addr => addr.Country)
        //      .WithMany(c => c.Addresses)
        //      .HasForeignKey(addr => addr.CountryId);

        //     builder.HasOne<AddressType>(addr => addr.AddressType)
        //      .WithMany(at => at.Addresses)
        //      .HasForeignKey(addr => addr.AddressTypeId);
        // }

        public void CreateTableData(EntityTypeBuilder<Application> builder) 
        {
            var dataArr = new List<Application>();
            dataArr.Add(new Application { ApplicationId = 1, Name = "Application1", Description = "Description1", Active = true });
            dataArr.Add(new Application { ApplicationId = 2, Name = "Application2", Description = "Description2", Active = true });

            foreach (var r in dataArr)
            {
                r.CreatedOn = new DateTime(2026, 2, 26, 0, 0, 0);
                r.CreatedBy = "dmaukTest";//ConfigurationConstants.DefaultCreatedBy;
                r.UpdatedOn = new DateTime(2026, 2, 26, 0, 0, 0);
                r.UpdatedBy = "dmaukTest";//ConfigurationConstants.DefaultCreatedBy;
            }

            builder.HasData(dataArr);

            //return dataArr;
        }

        // public Application CreateApplicationRecord(Application req)
        // {
        //     req.ApplicationId = _configCommon.CreadGuidForId(req.ApplicationId);
        //     return req;
        // }
}

