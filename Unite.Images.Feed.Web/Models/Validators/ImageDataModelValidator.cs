using FluentValidation;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Base.Validators;

namespace Unite.Images.Feed.Web.Models.Validators;

public class ImageDataModelValidator : AbstractValidator<ImageDataModel>
{
    private readonly IValidator<MriImageModel> _mriImageModelValidator;
    private readonly IValidator<CtImageModel> _ctImageModelValidator;
    private readonly IValidator<AnalysisModel> _imageAnalysisModelValidator;

    public ImageDataModelValidator()
    {
        _mriImageModelValidator = new MriImageModelValidator();
        _ctImageModelValidator = new CtImageModelValidator();
        _imageAnalysisModelValidator = new AnalysisValidator();


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
    private readonly IValidator<ImageDataModel> _imageModelValidator;


    public ImageModelsValidator()
    {
        _imageModelValidator = new ImageDataModelValidator();


        RuleForEach(model => model)
            .SetValidator(_imageModelValidator);
    }
}
