using System.Collections.Generic;
using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Images.Validators
{
    public class ImageModelValidator : AbstractValidator<ImageModel>
    {
        private readonly IValidator<MriImageModel> _mriImageModelValidator;
        private readonly IValidator<CtImageModel> _ctImageModelValidator;
        private readonly IValidator<AnalysisModel> _imageAnalysisModelValidator;

        public ImageModelValidator()
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


        private bool HaveModelSet(ImageModel model)
        {
            return model.MriImage != null
                || model.CtImage != null;
        }
    }


    public class ImageModelsValidator : AbstractValidator<ImageModel[]>
    {
        private readonly IValidator<ImageModel> _imageModelValidator;


        public ImageModelsValidator()
        {
            _imageModelValidator = new ImageModelValidator();


            RuleForEach(model => model)
                .SetValidator(_imageModelValidator);
        }
    }
}
