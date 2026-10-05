namespace Shared.Models.Dtos.CommonNote;

public record CommonNoteDto : AuditableDto
{
    public int NoteId { get; set; }
    public string NoteType { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Text { get; set; } = null!;
}