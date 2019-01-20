using FluentValidation;
using TIKSN.Configuration.Validator;

namespace TIKSN.Settings
{
    public class FileSettingsServiceOptionsValidator : PartialConfigurationFluentValidatorBase<FileSettingsServiceOptions>
    {
        public FileSettingsServiceOptionsValidator()
        {
            RuleFor(x => x.RelativePath).NotNull().NotEmpty();
        }
    }
}