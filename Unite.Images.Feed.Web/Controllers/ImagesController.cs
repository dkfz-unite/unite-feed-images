using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Extensions;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Models.Images;
using Unite.Images.Feed.Web.Models.Images.Converters;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Feed.Web.Services.Validation;

namespace Unite.Images.Feed.Web.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<ImageModel>> _validator;
        private readonly ImageDataWriter _dataWriter;
        private readonly ImageIndexingTasksService _indexingTasksService;
        private readonly ILogger _logger;

        private readonly ImageModelConverter _converter;


        public ImagesController(
            IValidationService validationService,
            IValidator<IEnumerable<ImageModel>> validator,
            ImageDataWriter dataWriter,
            ImageIndexingTasksService indexingTasksService,
            ILogger<ImagesController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataWriter = dataWriter;
            _indexingTasksService = indexingTasksService;
            _logger = logger;

            _converter = new ImageModelConverter();
        }


        public IActionResult Post([FromBody] ImageModel[] models)
        {
            if (!_validationService.ValidateParameter(models, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            models.ForEach(model => model.Sanitise());

            var dataModels = models.Select(model => _converter.Convert(model));

            _dataWriter.SaveData(dataModels, out var audit);

            _logger.LogInformation(audit.ToString());

            _indexingTasksService.PopulateTasks(audit.Images);

            return Ok();
        }
    }
}
