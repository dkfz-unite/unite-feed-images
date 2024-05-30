using FluentValidation;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Base.Validators;

namespace Unite.Images.Feed.Web.Models.Validators;

public class ImageModelValidator : AbstractValidator<ImageModel>
{
    private readonly IValidator<MriImageModel> _mriImageModelValidator = new MriImageModelValidator();
    private readonly IValidator<CtImageModel> _ctImageModelValidator = new CtImageModelValidator();

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

        RuleFor(model => model)
            .Must(HaveModelSet)
            .WithMessage("Specific image data (MriImage or CtImage) has to be set");

        RuleFor(model => model.MriImage)
            .SetValidator(_mriImageModelValidator);

        RuleFor(model => model.CtImage)
            .SetValidator(_ctImageModelValidator);
    }

    private bool HaveModelSet(ImageModel model)
    {
        return model.MriImage != null
            || model.CtImage != null;
    }
}


public class ImageModelsValidator : AbstractValidator<ImageModel[]>
{
    private readonly IValidator<ImageModel> _modelValidator = new ImageModelValidator();

    public ImageModelsValidator()
    {
        RuleFor(models => models)
            .Must(models => models.Any())
            .WithMessage("Should not be empty");

        RuleForEach(models => models)
            .SetValidator(_modelValidator);
    }
}
