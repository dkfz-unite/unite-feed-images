using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Analysis.Enums;
using Unite.Data.Entities.Images.Analysis.Radiomics;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Images;
using Unite.Indices.Entities.Basic.Images.Radiomics;

namespace Unite.Images.Indices.Services.Mapping;

public class ImageIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="enrollmentDate">Enrollment date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Image entity, DateOnly? enrollmentDate) where T : ImageIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index, enrollmentDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="enrollmentDate">Enrollment date (anchor date for calculation of relative days).</param>
    public static void Map(in Image entity, ImageIndex index, DateOnly? enrollmentDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.Type = entity.TypeId.ToDefinitionString();

        index.Mr = CreateFromMr(entity, enrollmentDate);
    }


    private static MrImageIndex CreateFromMr(in Image entity, DateOnly? enrollmentDate)
    {
        if (entity == null)
        {   
            return null;
        }

        return new MrImageIndex
        {
            Id = entity.Id,
            ReferenceId = entity.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(enrollmentDate),

            WholeTumor = entity.MrImage.WholeTumor,
            ContrastEnhancing = entity.MrImage.ContrastEnhancing,
            NonContrastEnhancing = entity.MrImage.NonContrastEnhancing,

            MedianAdcTumor = entity.MrImage.MedianAdcTumor,
            MedianAdcCe = entity.MrImage.MedianAdcCe,
            MedianAdcEdema = entity.MrImage.MedianAdcEdema,

            MedianCbfTumor = entity.MrImage.MedianCbfTumor,
            MedianCbfCe = entity.MrImage.MedianCbfCe,
            MedianCbfEdema = entity.MrImage.MedianCbfEdema,

            MedianCbvTumor = entity.MrImage.MedianCbvTumor,
            MedianCbvCe = entity.MrImage.MedianCbvCe,
            MedianCbvEdema = entity.MrImage.MedianCbvEdema,

            MedianMttTumor = entity.MrImage.MedianMttTumor,
            MedianMttCe = entity.MrImage.MedianMttCe,
            MedianMttEdema = entity.MrImage.MedianMttEdema,

            RadiomicsFeatures = CreateFrom(entity.Samples?.FirstOrDefault(sample => sample.Analysis.TypeId == AnalysisType.RFE)?.RadiomicsFeatureEntries)
        };
    }

    private static FeatureEntryIndex[] CreateFrom(in IEnumerable<FeatureEntry> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new FeatureEntryIndex
            {
                Feature = entity.Entity.Name,
                Value = entity.Value
            };

        }).ToArray();
    }
}
