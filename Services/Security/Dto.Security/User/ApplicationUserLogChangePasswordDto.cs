namespace Dto.Security.User
{
    public record UserLogChangePasswordDto
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public string OldPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}