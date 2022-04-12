using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Images.Features.Enums;

namespace Unite.Images.Feed.Web.Models.Images
{
    public class AnalysisModel
    {
        public string Id { get; set; }
        public AnalysisType? Type { get; set; }
        public DateTime? Date { get; set; }

        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, string> Features { get; set; }


        public void Sanitise()
        {
            Id = Id?.Trim();

            Parameters = Parameters?.ToDictionary(parameter => parameter.Key.Trim(), parameter => parameter.Value.Trim());
            Features = Features?.ToDictionary(feature => feature.Key.Trim(), feature => feature.Value.Trim());
        }
    }
}
