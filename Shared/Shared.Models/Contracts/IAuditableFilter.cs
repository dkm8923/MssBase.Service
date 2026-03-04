using System;

namespace Shared.Models.Contracts;

public interface IAuditableFilter
{
    string? CreatedBy { get; set; }
    DateOnly? CreatedOnDate { get; set; }
    string? UpdatedBy { get; set; }
    DateOnly? UpdatedOnDate { get; set; }
}
