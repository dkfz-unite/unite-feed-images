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
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Image entity, DateOnly? diagnosisDate) where T : ImageIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index, diagnosisDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    public static void Map(in Image entity, ImageIndex index, DateOnly? diagnosisDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.Type = entity.TypeId.ToDefinitionString();

        index.Mri = CreateFromMri(entity, diagnosisDate);
    }


    private static MriImageIndex CreateFromMri(in Image entity, DateOnly? diagnosisDate)
    {
        if (entity == null)
        {   
            return null;
        }

        return new MriImageIndex
        {
            Id = entity.Id,
            ReferenceId = entity.ReferenceId,
            CreationDay = entity.CreationDay ?? entity.CreationDate?.RelativeFrom(diagnosisDate),

            WholeTumor = entity.MriImage.WholeTumor,
            ContrastEnhancing = entity.MriImage.ContrastEnhancing,
            NonContrastEnhancing = entity.MriImage.NonContrastEnhancing,

            MedianAdcTumor = entity.MriImage.MedianAdcTumor,
            MedianAdcCe = entity.MriImage.MedianAdcCe,
            MedianAdcEdema = entity.MriImage.MedianAdcEdema,

            MedianCbfTumor = entity.MriImage.MedianCbfTumor,
            MedianCbfCe = entity.MriImage.MedianCbfCe,
            MedianCbfEdema = entity.MriImage.MedianCbfEdema,

            MedianCbvTumor = entity.MriImage.MedianCbvTumor,
            MedianCbvCe = entity.MriImage.MedianCbvCe,
            MedianCbvEdema = entity.MriImage.MedianCbvEdema,

            MedianMttTumor = entity.MriImage.MedianMttTumor,
            MedianMttCe = entity.MriImage.MedianMttCe,
            MedianMttEdema = entity.MriImage.MedianMttEdema,

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
