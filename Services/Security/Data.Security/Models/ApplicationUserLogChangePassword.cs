using Shared.Models.Contracts;

namespace Data.Security.Models;

public partial class ApplicationUserLogChangePassword : ICreateable
{
    public int LogId { get; set; }

    public int ApplicationUserId { get; set; }
    public int ApplicationId { get; set; }
    public string OldPassword { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public virtual Application Application { get; set; } = null!;
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
}
