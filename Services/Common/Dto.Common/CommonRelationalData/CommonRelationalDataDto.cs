using System.Text.Json.Serialization;
using Shared.Models;

namespace Dto.Common.CommonRelationalData
{
    public record CommonRelationalDataDto : AuditableDto
    {
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Value { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
    }
}