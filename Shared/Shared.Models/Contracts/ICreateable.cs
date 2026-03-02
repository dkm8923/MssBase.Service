namespace Shared.Models.Contracts
{
    public interface ICreateable
    {
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
    }
}
