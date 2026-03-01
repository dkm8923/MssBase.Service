using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Models;

namespace Shared.Data;

public static class DataUtilities
{   
    /// <summary>
    /// Configures the audit fields for the entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    public static void ConfigureAuditFields<T>(EntityTypeBuilder<T> builder) where T : AuditableEntity
    {
        builder.Property(t => t.CreatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.UpdatedOn).HasPrecision(2).IsRequired();
        builder.Property(t => t.UpdatedBy).HasMaxLength(64).IsRequired().IsUnicode(false);
        builder.Property(t => t.Active).IsRequired();
    }

    /// <summary>
    /// Sets the audit fields (CreatedOn, CreatedBy, UpdatedOn, UpdatedBy) for a collection of entities.
    /// </summary>
    public static void SetAuditFields<T>(IEnumerable<T> entities) where T : class
	{
		foreach (var entity in entities)
		{
			var type = typeof(T);
			var createdOnProp = type.GetProperty("CreatedOn");
			var createdByProp = type.GetProperty("CreatedBy");
			var updatedOnProp = type.GetProperty("UpdatedOn");
			var updatedByProp = type.GetProperty("UpdatedBy");
            
            if (createdOnProp != null && createdOnProp.CanWrite) 
            {
                createdOnProp.SetValue(entity, DataConstants.DefaultCreatedOn);
            }
				
			if (createdByProp != null && createdByProp.CanWrite) 
            {
                createdByProp.SetValue(entity, DataConstants.DefaultCreatedBy);
            }
				
			if (updatedOnProp != null && updatedOnProp.CanWrite) 
            {
                updatedOnProp.SetValue(entity, DataConstants.DefaultUpdatedOn);
            }
				
            if (updatedByProp != null && updatedByProp.CanWrite) 
            {
                updatedByProp.SetValue(entity, DataConstants.DefaultUpdatedBy);
            }
		}
	}

    /// <summary>
    /// Creates a unique key name based on the table name and key name. IE: "UQ_TableName_KeyName"  
    /// </summary>
    public static string CreateUniqueKey(string tableName, string keyName)
    {
        return $"UQ_{tableName}_{keyName}";
    }

    /// <summary>
    /// Creates a foreign key name based on the table name and key name. IE: "FK_TableName_KeyName"
    /// </summary>
    public static string CreateForeignKey(string tableName, string keyName)
    {
        return $"FK_{tableName}_{keyName}";
    }
}
