
using FluentValidation;
using Shared.Logic.Common;
using Shared.Models.Dtos.CommonNote;

namespace Shared.Logic.Validators;

public class InsertUpdateCommonNoteRequestValidator : AbstractValidator<InsertUpdateCommonNoteRequest>
{
    public InsertUpdateCommonNoteRequestValidator()
    {
        // Set cascade mode per rule (stops after first failure within each RuleFor)
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.NoteType)
            .Length(0, 32).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.NoteType, 32));
        
        RuleFor(v => v.Subject)
            .Length(0, 512).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Subject, 512));

        RuleFor(v => v.Text)
            .Length(0, 4096).WithMessage(ValidatorUtilities.CreateMaxLengthErrorMessage(Constants.EntityFieldNames.Text, 4096));

        RuleFor(v => v.CurrentUser).ValidateCurrentUser();
    }
}