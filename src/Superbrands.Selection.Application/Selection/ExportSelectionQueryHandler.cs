using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Superbrands.Libs.RestClients.FileStorage;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Libs.RestClients.PIM;
using Superbrands.Selection.Application.Options;
using Superbrands.Selection.Application.Selection.Export;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Application.Selection
{
    class ExportSelectionQueryHandler : IRequestHandler<ExportSelectionQuery, FileContentResult>
    {
        private readonly ISelectionRepository _selectionRepository;
        private readonly IPimProductsClient _pimProductsClient;
        private readonly IPimCategoriesClient _pimCategoriesClient;
        private readonly IFileStorageClient _fileStorageClient;
        private readonly ILogger<ExportSelectionQueryHandler> _logger;
        private readonly ExportOptions _exportOptions;

        public ExportSelectionQueryHandler(ISelectionRepository selectionRepository, IPimProductsClient pimProductsClient,
            IPimCategoriesClient pimCategoriesClient,
            IFileStorageClient fileStorageClient, IOptions<ExportOptions> exportOptions,
            ILogger<ExportSelectionQueryHandler> logger)
        {
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
            _pimProductsClient = pimProductsClient ?? throw new ArgumentNullException(nameof(pimProductsClient));
            _pimCategoriesClient = pimCategoriesClient;
            _fileStorageClient = fileStorageClient ?? throw new ArgumentNullException(nameof(fileStorageClient));
            _logger = logger;
            _exportOptions = exportOptions.Value ?? throw new ArgumentNullException(nameof(exportOptions));
        }

        public async Task<FileContentResult> Handle(ExportSelectionQuery request, CancellationToken cancellationToken)
        {
            var selection = await _selectionRepository.GetById(request.SelectionId, cancellationToken);

            if (selection == null)
                throw new Exception("Selection not found");

            var productSkus = selection.ColorModelMetas.SelectMany(z => z.Sizes).Select(x => x.Sku)
                .Distinct();

            var products = await _pimProductsClient.DealsAsync(null, productSkus, cancellationToken);
            var filesIds = products.SelectMany(x => x.ParentProduct.ProductFiles.Where(pf => pf.IsMain).Select(pf => pf.FileId))
                .Distinct();
            var categoriesIds = products.Select(x => x.CategoryId.Value).Distinct().ToList();

            var categoriesDict =
                (await _pimCategoriesClient.ByIdsAsync(categoriesIds, cancellationToken)).ToDictionary(x => x.Id);
            var imagesDict = await GetImages(filesIds, cancellationToken);

            var exportGenerator =
                new SelectionExportGenerator(selection.ToDomain(), products, _exportOptions, imagesDict, categoriesDict);
            return new FileContentResult(exportGenerator.Generate(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{ToUnicode(selection.Id.ToString())}_{DateTime.Now}.xlsx"
            };
        }

        private async Task<Dictionary<int, FileStreamResult>> GetImages(IEnumerable<int> filesIds,
            CancellationToken cancellationToken)
        {
            var imagesTasks = filesIds.Select(id => new
            {
                Id = id,
                Task = _fileStorageClient.V2GetFile(id, cancellationToken)
            }).ToDictionary(x => x.Id, x => x.Task);

            try
            {
                await Task.WhenAll(imagesTasks.Values);
            }
            catch
            {
                var ids = imagesTasks.Where(x => x.Value.Status == TaskStatus.Faulted).Select(x => x.Key);
                _logger.LogWarning($"Cannot load images with ids ({string.Join(",", ids)})");
            }

            return imagesTasks.Where(x => x.Value.Status != TaskStatus.Faulted).ToDictionary(x => x.Key, x => x.Value.Result);
        }

        private string ToUnicode(string str)
        {
            var latUp = new[]
            {
                "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F",
                "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya"
            };
            var latLow = new[]
            {
                "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f",
                "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya"
            };
            var rusUp = new[]
            {
                "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�",
                "�", "�", "�", "�", "�", "�", "�", "�", "�", "�"
            };
            var rusLow = new[]
            {
                "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�",
                "�", "�", "�", "�", "�", "�", "�", "�", "�", "�"
            };
            for (var i = 0; i <= 32; i++)
            {
                str = str.Replace(rusUp[i], latUp[i]);
                str = str.Replace(rusLow[i], latLow[i]);
            }

            return str.Replace(" ", "_");
        }
    }
}