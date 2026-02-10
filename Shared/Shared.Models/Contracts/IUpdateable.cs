namespace Shared.Models.Contracts
{
    public interface IUpdateable
    {
        string UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}
