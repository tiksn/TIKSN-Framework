using FluentValidation;
using TIKSN.Configuration.Validator;

namespace TIKSN.Settings;

public class FileSettingsServiceOptionsValidator : FluentValidateOptions<FileSettingsServiceOptions>
{
    public FileSettingsServiceOptionsValidator() => this.RuleFor(x => x.RelativePath).NotNull().NotEmpty();
}
