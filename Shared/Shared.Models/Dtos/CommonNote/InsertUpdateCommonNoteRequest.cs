namespace Shared.Models.Dtos.CommonNote;

public record InsertUpdateCommonNoteRequest
{
    public int? NoteId { get; set; }
    public required string NoteType { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
    public bool Active { get; set; }
}