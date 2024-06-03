using FluentValidation;

namespace Unite.Images.Feed.Web.Models.Radiomics.Validators;

public class AnalysisModelValidator : Base.Validators.AnalysisModelValidator<FeatureModel, FeatureModelValidator>
{
}

public class AnalysisModelsValidator : AbstractValidator<AnalysisModel[]>
{
    private readonly AnalysisModelValidator _modelValidator = new();

    public AnalysisModelsValidator()
    {
        RuleForEach(model => model)
            .SetValidator(_modelValidator);
    }
}
