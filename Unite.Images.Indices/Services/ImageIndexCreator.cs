using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Repositories.Constants;
using Unite.Data.Context.Repositories.Extensions.Queryable;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Images.Indices.Services.Mapping;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Images;

using SM = Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using CNV = Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Images.Indices.Services;

public class ImageIndexCreator
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly DonorsRepository _donorsRepository;
    private readonly SpecimensRepository _specimensRepository;
    private readonly SamplesRepository _samplesRepository;


    public ImageIndexCreator(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _donorsRepository = new DonorsRepository(dbContextFactory);
        _specimensRepository = new SpecimensRepository(dbContextFactory);
        _samplesRepository = new SamplesRepository(dbContextFactory);
    }


    public ImageIndex CreateIndex(object key)
    {
        var imageId = (int)key;

        return CreateImageIndex(imageId);
    }


    private ImageIndex CreateImageIndex(int imageId)
    {
        var image = LoadImage(imageId);

        if (image == null)
            return null;

        return CreateImageIndex(image);
    }

    private ImageIndex CreateImageIndex(Image image)
    {
        var enrollmentDate = image.Donor.ClinicalData?.EnrollmentDate;

        var index = new ImageIndex();

        ImageIndexMapper.Map(image, index, enrollmentDate);

        index.Donor = CreateDonorIndex(image.DonorId);
        index.Specimens = CreateSpecimenIndices(image.DonorId, enrollmentDate);
        index.Stats = CreateStatsIndex(image.TypeId, image.DonorId);
        index.Data = CreateDataIndex(image.TypeId, image.DonorId);

        return index;
    }

    private Image LoadImage(int imageId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Image>()
            .AsNoTracking()
            .IncludeMrImage()
            .IncludeRadiomicsFeatures()
            .Include(image => image.Donor)
                .ThenInclude(donor => donor.ClinicalData)
            .FirstOrDefault(image => image.Id == imageId);
    }


    private DonorIndex CreateDonorIndex(int donorId)
    {
        var donor = LoadDonor(donorId);

        if (donor == null)
            return null;

        return CreateDonorIndex(donor);
    }

    private static DonorIndex CreateDonorIndex(Donor donor)
    {
        return DonorIndexMapper.CreateFrom<DonorIndex>(donor);
    }

    private Donor LoadDonor(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Donor>()
            .AsNoTracking()
            .FirstOrDefault(donor => donor.Id == donorId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? enrollmentDate)
    {
        var specimens = LoadSpecimens(donorId);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen, enrollmentDate)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? enrollmentDate)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, enrollmentDate);

        index.Samples = CreateSampleIndices(specimen.Id, enrollmentDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? enrollmentDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, enrollmentDate)).ToArrayOrNull();
    }

    private SampleIndex CreateSampleIndex(Sample sample, DateOnly? enrollmentDate)
    {
        var index = SampleIndexMapper.CreateFrom<SampleIndex>(sample, enrollmentDate);

        var availability = _samplesRepository.HasRelatedOmicsResources(sample.Id).Result;

        if (availability != null)
        {
            index.Data = new Unite.Indices.Entities.Basic.Analysis.SampleDataIndex
            {
                Sm = availability.Sm,
                Cnv = availability.Cnv,
                Sv = availability.Sv,
                Cnvp = availability.Cnvp,
                Meth = availability.Meth,
                Exp = availability.GeneExp,
                ExpSc = availability.GeneExpSc,
                Prot = availability.ProtExp
            };
        }

        index.Resources = sample.Resources?.Select(resource => ResourceIndexMapper.CreateFrom<Unite.Indices.Entities.Basic.Analysis.ResourceIndex>(resource)).ToArray();

        return index;
    }

    private Sample[] LoadSamples(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => sample.SpecimenId == specimenId)
            .Where(sample => 
                sample.SmEntries.Any() ||
                sample.CnvEntries.Any() ||
                sample.SvEntries.Any() ||
                sample.GeneExpressions.Any() ||
                sample.Resources.Any() ||
                sample.ProteinExpressions.Any())
            .ToArray();
    }


    private StatsIndex CreateStatsIndex(ImageType type, int donorId)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;
        var geneIds = _specimensRepository.GetVariantRelatedGenes(specimenIds).Result;
        var smIds = _specimensRepository.GetRelatedVariants<SM.Variant>(specimenIds).Result;
        var cnvIds = _specimensRepository.GetRelatedVariants<CNV.Variant>(specimenIds).Result;
        var svIds = _specimensRepository.GetRelatedVariants<SV.Variant>(specimenIds).Result;
        
        return new StatsIndex
        {
            Genes = geneIds.Length,
            Sms = smIds.Length,
            Cnvs = cnvIds.Length,
            Svs = svIds.Length
        };
    }

    private DataIndex CreateDataIndex(ImageType type, int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorId),
            Treatments = CheckTreatments(donorId),
            Mrs = type == ImageType.MR,
            Cts = type == ImageType.CT,
            Materials = CheckSpecimen(donorId),
            MaterialsMolecular = CheckMolecularData(donorId),
            Sms = _specimensRepository.HaveVariants<SM.VariantEntry, SM.Variant>(specimenIds).Result,
            Cnvs = _specimensRepository.HaveVariants<CNV.VariantEntry, CNV.Variant>(specimenIds).Result,
            Svs = _specimensRepository.HaveVariants<SV.VariantEntry, SV.Variant>(specimenIds).Result,
            Cnvps = _specimensRepository.HaveProfiles(specimenIds).Result,
            Meth = _specimensRepository.HaveMethylation(specimenIds).Result,
            Exp = _specimensRepository.HaveGeneExpressions(specimenIds).Result,
            ExpSc = _specimensRepository.HaveGeneExpressionsPerCells(specimenIds).Result,
            Prot = _specimensRepository.HaveProteinExpressions(specimenIds).Result
        };
    }


    private bool CheckClinicalData(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<ClinicalData>()
            .AsNoTracking()
            .Where(clinical => clinical.DonorId == donorId)
            .Any();
    }

    private bool CheckTreatments(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Treatment>()
            .AsNoTracking()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();
    }

    private bool CheckSpecimen(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .Any();
    }

    private bool CheckMolecularData(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.MolecularData != null)
            .Any();
    }
}
