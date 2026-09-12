namespace Shared.Models.Dtos;

public record CommonNoteDto : AuditableDto
{
    public int CommonNoteId { get; set; }
    public string NoteType { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
}
