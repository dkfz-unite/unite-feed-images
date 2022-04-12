using System;
using System.Collections.Generic;
using Unite.Data.Entities.Images.Features.Enums;

namespace Unite.Images.Feed.Data.Models
{
    public class AnalysisModel
    {
        public string ReferenceId { get; set; }
        public AnalysisType? Type { get; set; }
        public DateTime? Date { get; set; }

        public IEnumerable<ParameterModel> Parameters { get; set; }
        public IEnumerable<FeatureModel> Features { get; set; }
    }
}
