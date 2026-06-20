using System.Text.Json.Serialization;

namespace Dto.Common.CommonRelationalData
{
    public record FilterCommonRelationalDataDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonTitle { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonSex { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonEthnicity { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonGender { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonMaritalStatus { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonReligion { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PersonSexuality { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? AddressType { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? PhoneNumberType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? UsaState { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? UsaTimeZone { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CommonRelationalDataDto>? Country { get; set; }
    }
}