using global::Shared.Models;
using MssBase.UI.Components.Shared.DataTable.Models;

namespace MssBase.UI.Components.Shared.DataTable.DefaultColumns;

public static class DefaultDataTableColumns
{
    public static DataTableColumn<TItem> Active<TItem>() where TItem : AuditableDto =>
        new() { Header = "Active", Width = 120, Value = x => x.Active };

    public static DataTableColumn<TItem> CreatedOn<TItem>() where TItem : AuditableDto =>
        new() { Header = "Created On", Width = 180, Value = x => x.CreatedOn };

    public static DataTableColumn<TItem> CreatedBy<TItem>() where TItem : AuditableDto =>
        new() { Header = "Created By", Width = 140, Value = x => x.CreatedBy };

    public static DataTableColumn<TItem> UpdatedOn<TItem>() where TItem : AuditableDto =>
        new() { Header = "Updated On", Width = 180, Value = x => x.UpdatedOn };

    public static DataTableColumn<TItem> UpdatedBy<TItem>() where TItem : AuditableDto =>
        new() { Header = "Updated By", Width = 140, Value = x => x.UpdatedBy };

    // Created On / By and Updated On / By, in display order.
    public static IEnumerable<DataTableColumn<TItem>> Audit<TItem>() where TItem : AuditableDto =>
        new[] { CreatedOn<TItem>(), CreatedBy<TItem>(), UpdatedOn<TItem>(), UpdatedBy<TItem>() };
}
