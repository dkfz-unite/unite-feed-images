﻿using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Base.Validators;

public class MriImageModelValidator : AbstractValidator<MriImageModel>
{
    public MriImageModelValidator()
    {
        RuleFor(model => model.WholeTumor)
            .NotNull()
            .WithMessage("Should not be empty");

        RuleFor(model => model.ContrastEnhancing)
            .Must(value => value > 0)
            .When(model => model.ContrastEnhancing.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.NonContrastEnhancing)
            .Must(value => value > 0)
            .When(model => model.NonContrastEnhancing.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianAdcTumor)
            .Must(value => value > 0)
            .When(model => model.MedianAdcTumor.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianAdcCe)
            .Must(value => value > 0)
            .When(model => model.MedianAdcCe.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianAdcEdema)
            .Must(value => value > 0)
            .When(model => model.MedianAdcEdema.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianCbfTumor)
            .Must(value => value > 0)
            .When(model => model.MedianCbfTumor.HasValue)
            .WithMessage("Should be greater than 0");
        
        RuleFor(model => model.MedianCbfCe)
            .Must(value => value > 0)
            .When(model => model.MedianCbfCe.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianCbfEdema)
            .Must(value => value > 0)
            .When(model => model.MedianCbfEdema.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianCbvTumor)
            .Must(value => value > 0)
            .When(model => model.MedianCbvTumor.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianCbvCe)
            .Must(value => value > 0)
            .When(model => model.MedianCbvCe.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianCbvEdema)
            .Must(value => value > 0)
            .When(model => model.MedianCbvEdema.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianMttTumor)
            .Must(value => value > 0)
            .When(model => model.MedianMttTumor.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianMttCe)
            .Must(value => value > 0)
            .When(model => model.MedianMttCe.HasValue)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.MedianMttEdema)
            .Must(value => value > 0)
            .When(model => model.MedianMttEdema.HasValue)
            .WithMessage("Should be greater than 0");
    }
}
