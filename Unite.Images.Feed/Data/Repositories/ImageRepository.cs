using System;
using Unite.Data.Entities.Images;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories
{
    internal class ImageRepository
    {
        private readonly ImageRepositoryBase<MriImageModel> _mriImageRepository;


        public ImageRepository(DomainDbContext dbContext)
        {
            _mriImageRepository = new MriImageRepository(dbContext);
        }


        public Image Find(int donorId, ImageModel model)
        {
            if (model is MriImageModel mriImage)
            {
                return _mriImageRepository.Find(donorId, mriImage);
            }
            else
            {
                throw new NotImplementedException("Image type is not yet supported");
            }
        }

        public Image Create(int donorId, ImageModel model)
        {
            if (model is MriImageModel mriImage)
            {
                return _mriImageRepository.Create(donorId, mriImage);
            }
            else
            {
                throw new NotImplementedException("Image type is not yet supported");
            }
        }

        public void Update(ref Image image, in ImageModel imageModel)
        {
            if (imageModel is MriImageModel mriImage)
            {
                _mriImageRepository.Update(ref image, mriImage);
            }
            else
            {
                throw new NotImplementedException("Image type is not yet supported");
            }
        }
    }
}
