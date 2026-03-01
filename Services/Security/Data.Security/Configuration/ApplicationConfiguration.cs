using Data.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;

namespace Data.Security.Configuration;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    private readonly string _tableName = "Application";
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        SetTableName(builder);

        builder.Property(t => t.ApplicationId).IsRequired();

        DataUtilities.ConfigureAuditFields(builder);

        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);

        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        CreateTableData(builder); 
    }

    public void SetTableName(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable(_tableName);
    }

    public void CreatePrimaryKey(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(e => e.ApplicationId);
    }
    public void CreateUniqueKey(EntityTypeBuilder<Application> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "Name"));
    }

    public void CreateTableData(EntityTypeBuilder<Application> builder) 
    {
        var dataArr = new List<Application>();
        dataArr.Add(new Application { ApplicationId = 1, Name = "EOS", Description = "Enterprise Dispatch and Monitoring System for Logistic Operations", Active = true });
        dataArr.Add(new Application { ApplicationId = 2, Name = "EPC", Description = "Enterprise Financial System for Processing Pricing & Commissions", Active = true });
        dataArr.Add(new Application { ApplicationId = 3, Name = "EBS", Description = "Enterprise User Permission Management System", Active = true });
        dataArr.Add(new Application { ApplicationId = 4, Name = "Bet-t", Description = "Interchange Configuration Tool", Active = true });
        dataArr.Add(new Application { ApplicationId = 5, Name = "MyPortfolio", Description = "Agent Analytics / Reporting Portal", Active = true });
        dataArr.Add(new Application { ApplicationId = 6, Name = "AIME", Description = "Agent Management Platform", Active = true });

        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}