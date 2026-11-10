using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MssBase.UI.Components.Shared.DataTable.Models;

public class DataTableColumn<TItem>
{
    public required string Header { get; init; }

    // Pixel width; null lets the browser size the column.
    public int? Width { get; init; }

    // Cell value, also used for sorting. Not required when CellTemplate is set and Sortable is false.
    public Func<TItem, object?>? Value { get; init; }

    // Format string applied when the value is IFormattable (e.g. "g" for dates).
    public string? Format { get; init; }

    public bool Sortable { get; init; } = true;

    public SortDirection InitialSortDirection { get; init; } = SortDirection.None;

    // Overrides default cell rendering (e.g. for action buttons).
    public RenderFragment<TItem>? CellTemplate { get; init; }
}
