using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Radiomics.Validators;

public class FeatureModelValidator : AbstractValidator<FeatureModel>
{
    public FeatureModelValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Should not be empty");

        RuleFor(model => model.Name)
            .MaximumLength(255).WithMessage("Maximum length is 255"); 

        RuleFor(model => model.Value)
            .NotEmpty().WithMessage("Should not be empty");
    }
}
