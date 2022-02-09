﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories
{
    public class FeatureOccurrenceRepository
    {
        private readonly DomainDbContext _dbContext;
        private readonly FeatureRepository _featureRepository;


        public FeatureOccurrenceRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _featureRepository = new FeatureRepository(dbContext);
        }


        public FeatureOccurrence Find(int analysedImageId, FeatureModel model)
        {
            return _dbContext.Set<FeatureOccurrence>()
                .Include(entity => entity.Feature)
                .FirstOrDefault(entity =>
                    entity.AnalysedImageId == analysedImageId &&
                    entity.Feature.Name == model.Name
            );
        }

        public IEnumerable<FeatureOccurrence> CreateOrUpdate(int analysedImageId, IEnumerable<FeatureModel> models)
        {
            RemoveRedundant(analysedImageId, models);

            var created = CreateMissing(analysedImageId, models);

            var updated = UpdateExisting(analysedImageId, models);

            return Enumerable.Concat(created, updated);
        }

        public IEnumerable<FeatureOccurrence> CreateMissing(int analysedImageId, IEnumerable<FeatureModel> models)
        {
            var entitiesToAdd = new List<FeatureOccurrence>();

            foreach (var model in models)
            {
                var entity = Find(analysedImageId, model);

                if (entity == null)
                {
                    var featureId = _featureRepository.FindOrCreate(model.Name).Id;

                    entity = new FeatureOccurrence()
                    {
                        AnalysedImageId = analysedImageId,
                        FeatureId = featureId
                    };

                    Map(model, ref entity);

                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                _dbContext.AddRange(entitiesToAdd);
                _dbContext.SaveChanges();
            }

            return entitiesToAdd;
        }

        public IEnumerable<FeatureOccurrence> UpdateExisting(int analysedImageId, IEnumerable<FeatureModel> models)
        {
            var entitiesToUpdate = new List<FeatureOccurrence>();

            foreach (var model in models)
            {
                var entity = Find(analysedImageId, model);

                if (entity != null)
                {
                    Map(model, ref entity);

                    entitiesToUpdate.Add(entity);
                }
            }

            if (entitiesToUpdate.Any())
            {
                _dbContext.UpdateRange(entitiesToUpdate);
                _dbContext.SaveChanges();
            }

            return entitiesToUpdate;
        }

        public void RemoveRedundant(int analysedImageId, IEnumerable<FeatureModel> models)
        {
            var featureNames = models.Select(model => model.Name);

            var entitiesToRemove = _dbContext.Set<FeatureOccurrence>()
                .Include(entity => entity.Feature)
                .Where(entity => entity.AnalysedImageId == analysedImageId && !featureNames.Contains(entity.Feature.Name))
                .ToArray();

            if (entitiesToRemove.Any())
            {
                _dbContext.RemoveRange(entitiesToRemove);
                _dbContext.SaveChanges();
            }
        }


        private void Map(in FeatureModel model, ref FeatureOccurrence entity)
        {
            entity.Value = model.Value;
        }
    }
}
