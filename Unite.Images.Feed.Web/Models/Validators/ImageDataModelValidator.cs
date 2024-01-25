using FluentValidation;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Base.Validators;

namespace Unite.Images.Feed.Web.Models.Validators;

public class ImageDataModelValidator : AbstractValidator<ImageDataModel>
{
    private readonly IValidator<MriImageModel> _mriImageModelValidator = new MriImageModelValidator();
    private readonly IValidator<CtImageModel> _ctImageModelValidator = new CtImageModelValidator();
    private readonly IValidator<AnalysisModel> _imageAnalysisModelValidator = new AnalysisValidator();

    public ImageDataModelValidator()
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

        RuleFor(model => model)
            .Must(HaveModelSet)
            .WithMessage("Specific image data (MriImage or CtImage) has to be set");

        RuleFor(model => model.MriImage)
            .SetValidator(_mriImageModelValidator);

        RuleFor(model => model.CtImage)
            .SetValidator(_ctImageModelValidator);

        RuleFor(model => model.Analysis)
            .SetValidator(_imageAnalysisModelValidator);
    }

    private bool HaveModelSet(ImageDataModel model)
    {
        return model.MriImage != null
            || model.CtImage != null;
    }
}


public class ImageModelsValidator : AbstractValidator<ImageDataModel[]>
{
    private readonly IValidator<ImageDataModel> _modelValidator = new ImageDataModelValidator();

    public ImageModelsValidator()
    {
        RuleFor(models => models)
            .Must(models => models.Any())
            .WithMessage("Should not be empty");

        RuleForEach(models => models)
            .SetValidator(_modelValidator);
    }
}
