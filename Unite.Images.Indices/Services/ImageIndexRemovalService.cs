using Unite.Indices.Context;
using Unite.Indices.Entities.Images;

namespace Unite.Images.Indices.Services;

public class ImageIndexRemovalService
{
    private readonly IIndexService<ImageIndex> _indexService;


    public ImageIndexRemovalService(IIndexService<ImageIndex> indexService)
    {
        _indexService = indexService;
    }


    public void DeleteIndex(object key)
    {
        var id = key.ToString();

        _indexService.Delete(id);
    }
}
