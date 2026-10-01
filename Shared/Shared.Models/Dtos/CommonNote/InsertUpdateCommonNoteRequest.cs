namespace Shared.Models.Dtos.CommonNote;

public record InsertUpdateCommonNoteRequest
{
    public required string NoteType { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
}