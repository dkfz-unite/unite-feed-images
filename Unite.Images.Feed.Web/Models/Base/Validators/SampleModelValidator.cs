using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Base.Validators;

public class SampleModelValidator : AbstractValidator<SampleModel>
{
    public SampleModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.ImageId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.ImageType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.AnalysisDate)
            .Empty()
            .When(model => model.AnalysisDay.HasValue)
            .WithMessage("Either exact 'date' or relative 'day' should be set, not both");

        RuleFor(model => model.AnalysisDay)
            .Empty()
            .When(model => model.AnalysisDate.HasValue)
            .WithMessage("Either exact 'date' or relative 'day' should be set, not both");

        RuleFor(model => model.AnalysisDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.AnalysisDay.HasValue)
            .WithMessage("Should be greater than or equal to 1");



        // RuleFor(model => model.Entry)
        //     .NotEmpty()
        //     .WithMessage("Should not be empty");

        // RuleFor(model => model.Entry)
        //     .Must(features => features.Any(IsSet))
        //     .When(features => features != null)
        //     .WithMessage("At least one feature should be set");
    }


    private bool IsSet(KeyValuePair<string, string> property)
    {
        return !string.IsNullOrWhiteSpace(property.Key) && !string.IsNullOrWhiteSpace(property.Value);
    }
}
