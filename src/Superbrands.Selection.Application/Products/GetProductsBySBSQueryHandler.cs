using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Application.Selection;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using ProductData = Superbrands.Selection.Application.Responses.ProductData;

namespace Superbrands.Selection.Application.Products
{
    internal class GetProductsBySBSQueryHandler : IRequestHandler<GetProductsBySBSQuery, ICollection<ProductData>>
    {
        private readonly IProductMetaRepository _repository;
        private readonly IPimBbproductsClient _pimProductsClient;

        public GetProductsBySBSQueryHandler([NotNull] IProductMetaRepository productMetaRepository,
            [NotNull] IPimBbproductsClient pimProductsClient)
        {
            _repository = productMetaRepository ?? throw new ArgumentNullException(nameof(productMetaRepository));
            _pimProductsClient = pimProductsClient ?? throw new ArgumentNullException(nameof(pimProductsClient));
        }

        public async Task<ICollection<ProductData>> Handle(GetProductsBySBSQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.Get(request.ModelVendoreCodesSbs, request.ProcurementId, request.SelectionId,
                request.PurchaseKeyId, request.SalePointId, cancellationToken);
            var vendorCodes = request.ModelVendoreCodesSbs.Cast<object>().ToArray();
            var pimRequest = new GroupingFilterParametersRequest() {Filters = new Dictionary<string, ICollection<object>>()};
            pimRequest.Filters.Add("ModelVendorCodeSbs", vendorCodes);
            var pimProducts = await _pimProductsClient.SearchAsync(pimRequest, cancellationToken);

            var resultProducts = ConvertProducts(pimProducts);
            foreach (var pimProduct in resultProducts)
            {
                var sizesFromSelections = products.Where(p => p.ModelVendorCodeSbs == pimProduct.ModelVendorCodeSbs).SelectMany(md => md.Sizes);

                foreach (var colorFromPim in pimProduct.ColorLevel)
                {
                    var colors = products.Where(p => p.ModelVendorCodeSbs == pimProduct.ModelVendorCodeSbs);
                    var sizeCharts = colors
                        .Select(p => p.SizeChartId).Distinct().ToList();
                    if (sizeCharts.Count == 1)
                    {
                        colorFromPim.SizeChartId = sizeCharts.First();
                        colorFromPim.SizeChartCount = colors.First().SizeChartCount;
                    }

                    foreach (var sizeFromPim in colorFromPim.RangeSizeLevel)
                    {
                        sizeFromPim.Count = sizesFromSelections.FirstOrDefault(sz => sz.Sku == sizeFromPim.Sku)?.Count;
                    }
                }
            }
            return resultProducts;
        }

        private static List<ProductData> ConvertProducts(PagedResult_1OfProductData pimProducts)
        {
            var resultProducts = pimProducts.Results.Select(p => new ProductData
            {
                Activity = p.Activity,
                Attributes = p.Attributes,
                Brand = p.Brand,
                Colors = p.Colors,
                Currency = p.Currency,
                Description = p.Description,
                Seasons = p.Seasons,
                Tags = p.Tags,
                ActivityType = p.ActivityType,
                BrandId = p.BrandId,
                BwpCurrency = p.BwpCurrency,
                BwpMin = p.BwpMin,
                CapsuleProducts = p.CapsuleProducts,
                CategoryId = p.CategoryId,
                ColorLevel = p.ColorLevel.Select(c => new Superbrands.Selection.Application.Responses.ColorLevel
                {
                    Box = c.Box,
                    Color = c.Color,
                    Rgb = c.Rgb,
                    ColorCode = c.ColorCode,
                    ColorEng = c.ColorEng,
                    ColorId = c.ColorId,
                    FileIds = c.FileIds,
                    IsPublication = c.IsPublication,
                    VendorCode = c.VendorCode,
                    AssortmentGroupId = c.AssortmentGroupId,
                    RangeSizeLevel = c.RangeSizeLevel,
                    ColorModelVendorCodeSbs = c.ColorModelVendorCodeSbs
                }).ToList(),
                InSelections = p.InSelections,
                LookProducts = p.LookProducts,
                ModelName = p.ModelName,
                OrderType = p.OrderType,
                RecommendedProducts = p.RecommendedProducts,
                RrcMin = p.RrcMin,
                SizeRange = p.SizeRange,
                SupplyDate = p.SupplyDate,
                CarouselFileIds = p.CarouselFileIds,
                PurchaseKeyId = p.PurchaseKeyId,
                Age = p.Age,
                AgeId = p.AgeId,
                Sex = p.Sex,
                SexId = p.SexId,
                CarouselCompressedFileIds = p.CarouselCompressedFileIds,
                ModelVendorCodeSbs = p.ModelVendorCodeSbs,
                IsPreorder = p.IsPreorder
            }).ToList();
            return resultProducts;
        }
    }
}