namespace Shared.Models.Contracts;

public interface IPerson
{
    public string Email { get; set; }
    public string? Title { get; set; }
	public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? PreferredName { get; set; }
    public string? Suffix { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? TimeZone { get; set; }
}