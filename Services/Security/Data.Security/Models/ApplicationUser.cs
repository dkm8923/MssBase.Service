using System;
using System.Collections.Generic;

namespace Data.Security.Models;

public partial class ApplicationUser
{
    public int ApplicationUserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool Active { get; set; }

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Password { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public DateTime? LastPasswordChangeDate { get; set; }

    public DateTime? LastLockoutDate { get; set; }

    public short? FailedPasswordAttemptCount { get; set; }

    public int ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;
}
