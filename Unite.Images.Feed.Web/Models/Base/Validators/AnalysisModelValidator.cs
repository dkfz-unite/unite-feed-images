using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Base.Validators;

public class AnalysisModelValidator<T, TValidator> : AbstractValidator<AnalysisModel<T>>
    where T : class, new()
    where TValidator : AbstractValidator<T>, new()
{
    private readonly IValidator<SampleModel> _sampleModelValidator;
    private readonly IValidator<T> _entryModelValidator;


    public AnalysisModelValidator()
    {
        _sampleModelValidator = new SampleModelValidator();
        _entryModelValidator = new TValidator();

        RuleFor(model => model.Sample)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_sampleModelValidator);

        RuleForEach(model => model.Entries)
            .SetValidator(_entryModelValidator)
            .When(model => model.Entries != null);
    }
}
