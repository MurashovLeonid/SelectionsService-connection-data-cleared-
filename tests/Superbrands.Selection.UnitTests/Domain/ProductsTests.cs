using Moq;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Application.Products;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.DDD.EfCore.Mappings;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Application;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Domain.Selections;
using Xunit;

namespace Superbrands.Selection.UnitTests.Domain
{
    public class ProductsTests
    {
        public ProductsTests()
        {
            DDDEntitiesMappings.Configure();
        }
        
        [Fact]
        public  async Task GetProductIdsByGroupingParamsTest()
        {
            AppMappings.Configure();
            var searchElasticProductsRequestRep = new List<SearchProductsRequest>();
            var _procurementRepository = new Mock<IProcurementRepository>();
            var _pimClient = new Mock<IPimBbproductsClient>();
            _pimClient.Setup(x => x.SearchAsync(It.IsAny<SearchProductsRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SearchProductsRequest,  CancellationToken>((s, c) => AddPimRequest(searchElasticProductsRequestRep, s)).ReturnsAsync(new PagedResult_1OfProductData() {RowCount =1 });
            _procurementRepository.Setup(x => x.GetProductsByGroupingParameters(It.IsAny<GroupKeyType>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetColorModelsForPim());

            var handler = new GetProductCountByStatusesQueryHandler(_procurementRepository.Object, _pimClient.Object);
            await handler.Handle(new GetProductCountByStatusesQuery(new GroupingFilterParametersRequest { Filters = new Dictionary<string, 
            ICollection<object>>() }), CancellationToken.None);

            Assert.True(searchElasticProductsRequestRep.Count == 5);
            Assert.True(searchElasticProductsRequestRep[0].PageSize == 0);
            Assert.True(searchElasticProductsRequestRep[0].Filters["ModelVendorCodeSbs"].First().ToString() == "1");
            Assert.True(searchElasticProductsRequestRep[1].PageSize == 0);
            Assert.True(searchElasticProductsRequestRep[1].Filters["ModelVendorCodeSbs"].First().ToString() == "2");
            Assert.True(searchElasticProductsRequestRep[2].PageSize == 0);
            Assert.True(searchElasticProductsRequestRep[2].Filters["ModelVendorCodeSbs"].First().ToString() == "3");
            Assert.True(searchElasticProductsRequestRep[3].PageSize == 0);
            Assert.True(searchElasticProductsRequestRep[3].Filters["ModelVendorCodeSbs"].First().ToString() == "4");
            Assert.True(searchElasticProductsRequestRep[4].PageSize == 0);
            Assert.True(searchElasticProductsRequestRep[4].Filters["ModelVendorCodeSbs"].First().ToString() == "5");
        }

        private IEnumerable<ColorModelMetaDalDto> GetColorModelsForPim()
        {
            IEnumerable<ColorModelMeta> colorModels = new List<ColorModelMeta>()
            {
                new("1", 1, "11", ColorModelStatus.Archive,  ColorModelPriority.PriorityA,  "$"),
                new("2" ,2, "22", ColorModelStatus.Canceled, ColorModelPriority.PriorityA, "$"),
                new("3", 3, "33", ColorModelStatus.New, ColorModelPriority.PriorityA, "$"),
                new("4", 4, "44", ColorModelStatus.PriceChanged, ColorModelPriority.PriorityA, "$"),
                new("5", 5, "55", ColorModelStatus.QuantityChanged, ColorModelPriority.PriorityA, "$"),
            };
            return colorModels.Select(x=> ColorModelMetaDalDto.FromDomain(x));
        }

        private void AddPimRequest(List<SearchProductsRequest> searchElasticProductsRequestRep, SearchProductsRequest searchElasticProductsRequest)
        {
            if (searchElasticProductsRequest != null)
            {
                var s = new SearchProductsRequest()
                {
                    Filters = new Dictionary<string, ICollection<object>> { { "ModelVendorCodeSbs", searchElasticProductsRequest.Filters["ModelVendorCodeSbs"] } },
                    PageSize = searchElasticProductsRequest.PageSize
                };
                searchElasticProductsRequestRep.Add(s);
            }
        }


    }
}
