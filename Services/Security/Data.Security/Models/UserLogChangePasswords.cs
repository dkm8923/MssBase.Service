using Shared.Models.Contracts;

namespace Data.Security.Models;

public partial class UserLogChangePassword : ICreateable
{
    public int LogId { get; set; }

    public int UserId { get; set; }
    public string OldPassword { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public virtual User User { get; set; } = null!;
}
