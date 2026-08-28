using Shared.Models.Contracts;

namespace Data.Security.Models;

public partial class UserLogLogin : ICreateable
{
    public int LogId { get; set; }

    public int UserId { get; set; }
    public int ApplicationId { get; set; }
    public string AuthToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public virtual Application Application { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
