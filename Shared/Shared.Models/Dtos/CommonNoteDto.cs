namespace Shared.Models.Dtos;

public record CommonNoteDto : AuditableDto
{
    public int CommonNoteId { get; set; }
    public string NoteType { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Text { get; set; } = null!;
}