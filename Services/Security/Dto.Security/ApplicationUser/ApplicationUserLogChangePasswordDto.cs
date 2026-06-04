namespace Dto.Security.ApplicationUser
{
    public record ApplicationUserLogChangePasswordDto
    {
        public int ApplicationUserLogChangePasswordId { get; set; }
        public int ApplicationUserId { get; set; }
        public int ApplicationId { get; set; }
        public string OldPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}