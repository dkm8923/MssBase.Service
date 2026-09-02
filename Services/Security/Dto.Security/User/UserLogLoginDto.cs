namespace Dto.Security.User
{
    public record UserLogLoginDto
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}