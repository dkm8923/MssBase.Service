using Data.Security.Models;
using Shared.Logic.Common;
using Microsoft.EntityFrameworkCore;
using Dto.Security.User;
using System.Text.Json;
using Shared.Models.Dtos;
using Shared.Models.Dtos.CommonNote;

namespace Data.Security.Converters
{
    public static class UserConverters
    {
        public static UserDto ToDto(this User source)
        {
            if (source == null)
            {
                return null;
            }

            var applicationUserLogin = source.UserLogin != null ? source.UserLogin : new UserLogin();

            var target = new UserDto
            {
                UserId = source.UserId,
                Active = source.Active,
                ReadOnly = source.ReadOnly,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                Email = source.Email,
                Title = source.Title,
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                PreferredName = source.PreferredName,
                Suffix = source.Suffix,
                DateOfBirth = source.DateOfBirth,
                TimeZone = source.TimeZone,
                MaritalStatus = source.MaritalStatus,
                Religion = source.Religion,
                Gender = source.Gender,
                SpokenLanguages = source.SpokenLanguageJson == null ? null : JsonSerializer.Deserialize<List<string>>(source.SpokenLanguageJson),
                PhoneNumbers = source.PhoneNumberJson == null ? null : JsonSerializer.Deserialize<List<TypedValueDto>>(source.PhoneNumberJson),
                SocialMediaProfiles = source.SocialMediaProfileJson == null ? null : JsonSerializer.Deserialize<List<SocialMediaProfileDto>>(source.SocialMediaProfileJson),
                AlternateEmails = source.AlternateEmailJson == null ? null : JsonSerializer.Deserialize<List<TypedValueDto>>(source.AlternateEmailJson),
                GenderPronouns = source.GenderPronounJson == null ? null : JsonSerializer.Deserialize<List<string>>(source.GenderPronounJson),
                ImportantDates = source.ImportantDateJson == null ? null : JsonSerializer.Deserialize<List<TypedValueDto>>(source.ImportantDateJson),
                CommonNotes = source.CommonNoteJson == null ? null : JsonSerializer.Deserialize<List<CommonNoteDto>>(source.CommonNoteJson),
                Password = applicationUserLogin.Password,
                PasswordResetRequired = applicationUserLogin.PasswordResetRequired,
                LastLoginDateTime = applicationUserLogin.LastLoginDateTime,
                LastPasswordChangeDateTime = applicationUserLogin.LastPasswordChangeDateTime,
                LastLockoutDateTime = applicationUserLogin.LastLockoutDateTime,
                FailedPasswordAttemptCount = applicationUserLogin.FailedPasswordAttemptCount,
            };

            if (source.ApplicationUsers.NotNullAndHasRecords())
            {
                target.ApplicationUsers = source.ApplicationUsers.Select(au => au.ToDto());
            }

            return target;
        }

        public static UserDto ToDtoWithoutPassword(this User source)
        {
            if (source == null)
            {
                return null;
            }

            source.UserLogin.Password = null;
            return source.ToDto();
        }

        public static async Task<List<UserDto>> ToDtos(this IQueryable<User> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDto()).ToListAsync(cancellationToken);

            return target;
        }

        public static async Task<List<UserDto>> ToDtosWithoutPassword(this IQueryable<User> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                return null;
            }

            var target = await source.Select(src => src.ToDtoWithoutPassword()).ToListAsync(cancellationToken);

            return target;
        }

        public static User ToEntityOnInsert(this InsertUpdateUserRequest source)
        {
            if (source == null)
            {
                return null;
            }

            //format common note(s) on insert
            var commonNoteJson = null;
            
            if (req.CommonNotes != null)
            {
                var commonNotes = new List<CommonNoteDto>();
                var idx = 1;
                foreach (var commonNote in req.CommonNotes)
                {
                    commonNotes.Add(new CommonNoteDto {
                        CommonNoteId = idx,
                        NoteType = commonNote.NoteType,
                        Subject = commonNote.Subject,
                        Text = commonNote.Text,
                        commonNote.CreatedBy = req.CurrentUser,
                        commonNote.CreatedOn = DateTime.UtcNow
                    });
                    
                    idx++;
                }

                commonNoteJson = commonNotes.Count() > 0 ? JsonSerializer.Serialize(commonNotes) : null;
            }

            var target = new User
            {
                Active = source.Active,
                Email = source.Email,
                Title = source.Title,
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                PreferredName = source.PreferredName,
                Suffix = source.Suffix,
                DateOfBirth = source.DateOfBirth,
                TimeZone = source.TimeZone,
                MaritalStatus = source.MaritalStatus,
                Religion = source.Religion,
                Gender = source.Gender,
                SpokenLanguageJson = JsonSerializer.Serialize(source.SpokenLanguages),
                PhoneNumberJson = JsonSerializer.Serialize(source.PhoneNumbers),
                SocialMediaProfileJson = JsonSerializer.Serialize(source.SocialMediaProfiles),
                AlternateEmailJson = JsonSerializer.Serialize(source.AlternateEmails),
                GenderPronounJson = JsonSerializer.Serialize(source.GenderPronouns),
                ImportantDateJson = JsonSerializer.Serialize(source.ImportantDates),
                CommonNoteJson = commonNoteJson,
                CurrentUser = source.CurrentUser
            };

            return target;
        }

        public static User UpdateEntityFromRequest(this User entity, InsertUpdateUserRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.Email = source.Email;
            entity.Title = source.Title;
            entity.FirstName = source.FirstName;
            entity.MiddleName = source.MiddleName;
            entity.LastName = source.LastName;
            entity.PreferredName = source.PreferredName;
            entity.Suffix = source.Suffix;
            entity.DateOfBirth = source.DateOfBirth;
            entity.TimeZone = source.TimeZone;
            entity.CurrentUser = source.CurrentUser;

            return entity;
        }
    }
}
