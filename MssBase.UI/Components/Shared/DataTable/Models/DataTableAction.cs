using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

namespace MssBase.UI.Components.Shared.DataTable.Models;

public enum DataTableActionType
{
    Edit,
    Delete,
    View
}

public record DataTableAction<TItem>(DataTableActionType Type, Func<TItem, Task>? OnClick = null);

public static class DataTableColumn
{
    // Builds a column that renders the standard action buttons for each row.
    public static DataTableColumn<TItem> Actions<TItem>(params DataTableAction<TItem>[] actions)
    {
        return new DataTableColumn<TItem>
        {
            Header = "Action(s)",
            Width = 40 + (actions.Length * 40),
            Sortable = false,
            CellTemplate = item => (RenderTreeBuilder builder) =>
            {
                builder.OpenComponent<DataTableActionColumn<TItem>>(0);
                builder.AddAttribute(1, nameof(DataTableActionColumn<TItem>.Item), item);
                builder.AddAttribute(2, nameof(DataTableActionColumn<TItem>.Actions), actions);
                builder.CloseComponent();
            }
        };
    }
}
