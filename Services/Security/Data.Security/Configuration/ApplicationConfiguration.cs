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
        builder.ConfigureAuditFields();
        builder.Property(t => t.Name).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Description).HasMaxLength(256).IsUnicode(false);
        
        CreatePrimaryKey(builder);
        CreateUniqueKey(builder);
        //CreateTableData(builder); 
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
        dataArr.Add(new Application { ApplicationId = 1, Name = "MSS Security", Description = "Enterprise application security management for Mauk Software Solutions LLC.", Active = true, ReadOnly = true });
        
        DataUtilities.SetAuditFields(dataArr);

        builder.HasData(dataArr);
    }
}