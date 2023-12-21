using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Images.Validators;

public class AnalysisValidator : AbstractValidator<AnalysisModel>
{
    public AnalysisValidator()
    {
        RuleFor(model => model.Id)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.Day)
            .GreaterThanOrEqualTo(0)
            .When(model => model.Day.HasValue)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.Parameters)
            .Must(parameters => parameters.Any(IsSet))
            .When(parameters => parameters != null)
            .WithMessage("At least one parameter should be set");


        RuleFor(model => model.Features)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Features)
            .Must(features => features.Any(IsSet))
            .When(features => features != null)
            .WithMessage("At least one feature should be set");
    }


    private bool IsSet(KeyValuePair<string, string> property)
    {
        return !string.IsNullOrWhiteSpace(property.Key) && !string.IsNullOrWhiteSpace(property.Value);
    }
}
