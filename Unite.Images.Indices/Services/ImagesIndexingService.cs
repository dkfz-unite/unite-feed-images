using System.Linq.Expressions;
using Unite.Indices.Entities.Images;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Images.Indices.Services
{
    public class ImagesIndexingService : IndexingService<ImageIndex>
    {
        protected override string DefaultIndex
        {
            get { return "images"; }
        }

        protected override Expression<Func<ImageIndex, object>> IdProperty
        {
            get { return (image) => image.Id; }
        }


        public ImagesIndexingService(IElasticOptions options) : base(options)
        {
        }
    }
}
