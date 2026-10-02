using Shared.Data.Models;

namespace Data.Security.Models;

public class UserNote : CommonNote
{
    public virtual User User { get; set; } = null!;
}
