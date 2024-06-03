using FluentValidation;
using Unite.Images.Feed.Web.Models.Base;

namespace Unite.Images.Feed.Web.Models.Validators.Base;

public abstract class ImageModelValidator<TModel> : AbstractValidator<TModel>
    where TModel : ImageModel
{
    public ImageModelValidator()
    {
        RuleFor(model => model.Id)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Id)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.CreationDate)
            .Empty()
            .When(model => model.CreationDay.HasValue)
            .WithMessage("Either exact 'date' or relative 'day' should be set, not both");

        RuleFor(model => model.CreationDay)
            .Empty()
            .When(model => model.CreationDate.HasValue)
            .WithMessage("Either exact 'date' or relative 'day' should be set, not both");

        RuleFor(model => model.CreationDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.CreationDay.HasValue)
            .WithMessage("Should be greater than or equal to 1");
    }
}
