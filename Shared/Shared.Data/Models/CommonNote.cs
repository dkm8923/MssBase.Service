namespace Shared.Data.Models;

public partial class CommonNote : AuditableEntity
{
    public int CommonNoteId { get; set; }
    public string NoteType { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
}
