using MediatR;
using Microsoft.AspNetCore.Mvc;
using Superbrands.Selection.WebApi.DTOs;
using System.Collections.Generic;
using System.Net;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Superbrands.Selection.Domain.Enums;
using System.Threading;
using Superbrands.Selection.Application.Procurement;
using Superbrands.Selection.Infrastructure.RepositoryResponses;
using Superbrands.Selection.Application.Requests;
using System.Linq;
using Superbrands.Selection.Application.Products;
using Superbrands.Selection.Domain;
using Superbrands.Selection.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Superbrands.Selection.WebApi.Controllers
{
    //ToDo: уточнить нужен ли вообще этот mock
    [Route("api/v1/procurements")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProcurementsController : ControllerBase
    {
        public ProcurementsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(PagedResult<ProcurementDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Search([FromBody,Required] SearchProcurementRequest request, CancellationToken 
        cancellationToken)
        {
            var query = new SearchProcurementQuery(request.PartnersIds, request.SeasonCapsulesIds,
                request.SalePointsIds, request.BuyersIds, request.Page, request.Size);

            var pagedProcurements = await Mediator.Send(query, cancellationToken);
            return Ok(pagedProcurements.MapTo(x => new ProcurementDto(x)));
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(List<ProcurementDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GenerateProcurementsForPartners([FromBody] GenerateProcurementsForPartnersRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GenerateProcurementsForPartnersQuery(request?.PartnersIds, request?.SeasonCapsulesIds, HttpContext.GetOperationLog());
            var procurements = await Mediator.Send(query, cancellationToken);
            return Ok(procurements.Select(x => new ProcurementDto(x)));
        }

        [HttpPost("generate-by-sale-points")]
        [ProducesResponseType(typeof(List<ProcurementDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TestSalePointConsumer([FromBody] List<long> salePointIds,
            CancellationToken cancellationToken)
        {
            var query = new GenerateProcurementsForSalePointsQuery(new Libs.DDD.Abstractions.OperationLog("Consumer", "1", DateTime.Now), salePointIds);
            var procurements = await Mediator.Send(query, cancellationToken);
            return Ok(procurements.Select(x => new ProcurementDto(x)));
        }


        [HttpPost("generate-by-season")]
        [ProducesResponseType(typeof(List<ProcurementDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GenerateProcurementsForSeason(
            [FromBody, Required] GenerateProcurementsForSeasonRequest request, CancellationToken cancellationToken)
        {
            var query = new GenerateProcurementsForSeasonQuery(HttpContext.GetOperationLog(), request.SeasonCapsulesId);
            var procurements = await Mediator.Send(query, cancellationToken);
            return Ok(procurements.Select(x => new ProcurementDto(x)));
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(ProcurementDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProcurementById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var procurement = await Mediator.Send(new GetProcurementByIdQuery(id), cancellationToken);
            if (procurement == null)
                return NotFound();

            var procurementDto = new ProcurementDto(procurement);

            procurementDto.SalePoints = procurementDto.SalePoints
                .Where(s => s.IsActive)
                .Select(async s => new SalePointDto
                {
                    SelectedProductsCount =
                        await Mediator.Send(new GetProductsCountByProcurementAndSalePointIdsQuery(procurement.Id, s.Id),
                            cancellationToken),
                    Id = s.Id,
                    IsActive = s.IsActive
                }).Select(x => x.Result).ToList();

            return Ok(procurementDto);
        }

        [Route("{id}/set-brand-discount")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetBrandDiscount([FromRoute, Required] int id, [FromBody]BrandDiscountDto discount, CancellationToken cancellationToken)
        {
            var procurement = await Mediator.Send(new GetProcurementByIdQuery(id), cancellationToken);
            if (procurement == null)
                return NotFound($"No procurement with id {id}");

            procurement.SetBrandDiscount(discount.BrandId, discount.Discount);

            await Mediator.Send(new UpdateProcurementQuery(procurement, GetOperationLogFromContext()), cancellationToken);
            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<ProcurementDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProcurementByIds([FromQuery] IEnumerable<long> ids,
            CancellationToken cancellationToken)
        {
            var procurements = await Mediator.Send(new GetProcurementsByIdsQuery(ids), cancellationToken);
            var procurementDtos = procurements.Select(p =>
            {
                var dto = new ProcurementDto(p);
                dto.SalePoints = dto.SalePoints.Where(s => s.IsActive)
                    .Select(async s => new SalePointDto
                    {
                        SelectedProductsCount = await Mediator.Send(new GetProductsCountByProcurementAndSalePointIdsQuery(p.Id, s.Id), cancellationToken),
                        Id = s.Id,
                        IsActive = s.IsActive
                    })
                    .Select(x => x.Result)
                    .ToList();
                return dto;
            }).ToList();
            var result = OrderByIds(ids, procurementDtos);
            return Ok(result);
        }

        private static List<ProcurementDto> OrderByIds(IEnumerable<long> ids, List<ProcurementDto> procurementDtos)
        {
            var procurementsById = procurementDtos.ToDictionary(p => p.Id);
            var result = new List<ProcurementDto>();
            foreach (var id in ids)
            {
                if (procurementsById.TryGetValue(id, out var dto))
                {
                    result.Add(dto);
                }
            }

            return result;
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(IEnumerable<long>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FilterProductsByGroupingParameters([FromRoute] int id,
            [FromQuery] GroupingFilterParametersRequest groupingFilterParametersRequest,
            [FromBody] Dictionary<dynamic, object> filterParams, CancellationToken cancellationToken)
        {
            var query = new GetProductIdsByGroupingParamsQuery(id, groupingFilterParametersRequest, filterParams);
            var ids = await Mediator.Send(query, cancellationToken);
            return Ok(ids);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<long>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateProcurementForPartner(
            [FromBody] IEnumerable<CreateProcurementForPartnerRequest> createProcurementForPartnerRequest,
            CancellationToken cancellationToken)
        {
            var query = new CreateProcurementForPartnerQuery(createProcurementForPartnerRequest, int.Parse(GetCurrentUserId()));
            var createdProcurementIds = await Mediator.Send(query, cancellationToken);
            return Ok(createdProcurementIds);
        }

        [HttpPost("change-salePoint-status")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeSalePointStatus(int procurementId, int salePointId, bool isActive,
            CancellationToken cancellationToken)
        {
            var procurement = await Mediator.Send(new GetProcurementByIdQuery(procurementId), cancellationToken);
            if (procurement == null)
                return NotFound($"Procurement with id {procurementId} is not found");

            var salePoint = procurement.SalePoints.FirstOrDefault(sp => sp.Id == salePointId);
            if (salePoint == null)
                return NotFound($"Procurement with id {procurementId} does not contain salePoint with Id {salePointId}");

            if (isActive == false)
            {
                var query = new DeleteAllProductsBySalePointAndProcurementIdQuery(procurementId, salePointId);
                await Mediator.Send(query, cancellationToken);
            }

            salePoint.IsActive = isActive;
            await Mediator.Send(new UpdateProcurementQuery(procurement, HttpContext.GetOperationLog()),
                cancellationToken);
            return Ok();
        }

        [HttpPost("{id}/group")]
        [ProducesResponseType(typeof(PagedResult<GroupedSelectionDto>), 200)]
        public async Task<IActionResult> GetGroupedSelections([FromRoute] int id, [FromQuery] GroupKeyType groupKeyType,
            CancellationToken cancellationToken)
        {
            var btResponse = new BTSelectionsGroupResponse
            {
                MemberIds = new List<long> {1, 2, 3},
                PurchaseKeyDepartment = new List<PurchaseKeySalePointDto>
                {
                    new()
                    {
                        SalePointId = 1,
                        PurchaseKeyId = 1
                    }
                }
            };

            await Mediator.Send(new GroupSelectionsQuery(id, btResponse, groupKeyType), cancellationToken);

            var rand = new Random();
            var selectionGroupList = new PagedResult<GroupedSelectionDto>();
            selectionGroupList.Results = new List<GroupedSelectionDto>();

            for (int i = 0; i < rand.Next(3, 10); i++)
                selectionGroupList.Results.Add(new GroupedSelectionDto
                {
                    ProductsCount = rand.Next(1, 90000),
                    Coefficient = rand.NextDouble(),
                    Marginality = rand.NextDouble(),
                    AverageRrc = rand.NextDouble(),
                    AverageBwp = rand.NextDouble(),
                    ColorModelCount = rand.Next(1, 90000),
                    SizesCount = rand.Next(1, 90000),
                    ProcurementGroupKeyParameters = new GroupKeyParametersDto
                    {
                        PartnerId = rand.Next(1, 999),
                        SeasonCapsuleId = rand.Next(1, 999),
                        SeasonId = rand.Next(1, 999),
                        MemberIds = new List<int> {1, 2, 3}
                    }
                });

            return Ok(selectionGroupList);
        }

        [Route("group")]
        [HttpPost]
        [ProducesResponseType(typeof(PagedResult<ProcurementGroupDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSortFilterGroupProcurements([FromBody] GetProcurementGroupsRequest request,
            CancellationToken cancellationToken)
        {
            var rand = new Random();
            var btResponse = new BTSelectionsGroupResponse
            {
                MemberIds = new List<long> {1, 2, 3},
                PurchaseKeyDepartment = new List<PurchaseKeySalePointDto>
                {
                    new()
                    {
                        SalePointId = 1,
                        PurchaseKeyId = 1
                    }
                }
            };

            await Mediator.Send(new GroupProcurementsQuery(btResponse), cancellationToken);

            var procurementGroupList = new PagedResult<ProcurementGroupDto>();
            procurementGroupList.Results = new List<ProcurementGroupDto>();

            for (int i = 0; i < rand.Next(3, 10); i++)
            {
                procurementGroupList.Results.Add(new ProcurementGroupDto
                {
                    ProcurementId = rand.Next(1, 999),
                    ProcurementGroupMeta = new ProcurementGroupMetaDto
                    {
                        ProductsCount = rand.Next(1, 999),
                        AverageRrc = rand.Next(1, 999),
                        AverageBwp = rand.Next(1, 999),
                        ColorModelCount = rand.Next(1, 999),
                        Coefficient = rand.NextDouble(),
                        Marginality = rand.NextDouble(),
                        SizesCount = rand.Next(1, 999)
                    },
                    ProcurementGroupKeyParameters = new GroupKeyParametersDto
                    {
                        PartnerId = rand.Next(1, 999),
                        SeasonCapsuleId = rand.Next(1, 999),
                        SeasonId = rand.Next(1, 999),
                        MemberIds = new List<int> {1, 2, 3}
                    }
                });
            }

            return Ok(procurementGroupList);
        }

        [Route("{procurementId}/product-statistic")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductStatisticDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProductStatistic([FromRoute, Required] long procurementId, [FromQuery, Required] List<string> modelVendoreCodes, CancellationToken cancellationToken)
        {
            if (modelVendoreCodes?.Any() != true)
            {
                return Ok(new List<ProductStatisticDto>());
            }
            var colorModelMetas = await Mediator.Send(new GetProductStatisticQuery(procurementId, modelVendoreCodes));

            var productStatisticDtos = colorModelMetas.GroupBy(q => q.ModelVendorCodeSbs).Select(item => new ProductStatisticDto {
                ModelVendoreCodeSbs = item.Key,
                ColorModelVendoreCodesBySalePoints = item.GroupBy(itemcmm => itemcmm.ColorModelGroupKeys.SalePointId).Select(cmm => new ColorModelVendoreCodesBySalePoints
                {
                SalePointId = cmm.Key,
                ColorModelVendoreCodes = cmm.Select(q => q.ColorModelVendorCodeSbs).ToList()
                })
            });

            return Ok(productStatisticDtos);
        }
    }
}