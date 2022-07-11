using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Superbrands.Libs.RestClients.Pim;
using Superbrands.Selection.Application.Products;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Application.Selection;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.Helpers;
using Superbrands.Selection.WebApi.DTOs;
using ProductData = Superbrands.Selection.Application.Responses.ProductData;

namespace Superbrands.Selection.WebApi.Controllers
{
    [Route("api/v1/products/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProductsController : ControllerBase
    {
        private readonly IPimBbproductsClient _pimClient;

        public ProductsController(IMediator mediator, IPimBbproductsClient pimClient) :
            base(mediator)
        {
            _pimClient = pimClient ?? throw new ArgumentNullException(nameof(pimClient));
        }

        [HttpGet("/api/v1/selections/{selectionId}/products")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ProductsCreateDealsFormatDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetProductsBySelectionId([FromRoute, Required] long selectionId,
            CancellationToken cancellationToken)
        {
            if (selectionId <= 0)
                return BadRequest("selection id cannot be less than zero");

            var selection = await Mediator.Send(new GetSelectionByIdQuery(selectionId), cancellationToken);

            if (selection == null)
                return NoContent();

            return Ok(new ProductsCreateDealsFormatResponse(selection).Products);
        }

        [HttpPost("colorModelStatus/{colorModelStatus}")]
        public async Task<IActionResult> GetProductsByColorModelStatus([FromRoute, Required] ColorModelStatus
            colorModelStatus, [FromBody, Required] SearchProductsRequest request, CancellationToken cancellationToken)
        {
            var pimProducts = await Mediator.Send(new GetProductsByColorModelStatusQuery(colorModelStatus, request),
                cancellationToken);
            return Ok(pimProducts);
        }

        [HttpPost("/api/v1/selections/{selectionId}/products/search")]
        [ProducesResponseType(typeof(PagedResult_1OfProductData), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchSelectionsProduct([FromBody] SearchProductsRequest request,
            [FromRoute, Required] long selectionId, CancellationToken cancellationToken)
        {
            if (selectionId <= 0)
                return BadRequest("selection id cannot be less than zero");

            var selection = await Mediator.Send(new GetSelectionByIdQuery(selectionId), cancellationToken);
            if (selection == null)
                return NotFound("Selection not found");

            if (!selection.ColorModelMetas.Any())
                return Ok(new PagedResult_1OfProductData());

            var vendorCodes = selection.ColorModelMetas.Select(p => p.ModelVendorCodeSbs).ToList();
            request.Filters.Add("ModelVendorCodeSbs", vendorCodes.Cast<object>().ToArray());
            var pimProducts = await _pimClient.SearchAsync(request, cancellationToken);

            SelectionHelper.FilterOutProductsNotInSelection(selection.ColorModelMetas, pimProducts.Results);
            return Ok(pimProducts);
        }


        [Route("add")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddProductToSelections(
            [FromBody, Required] AddProductToSelectionRequest request, CancellationToken cancellationToken)
        {
            await Mediator.Send(
                new AddProductsToSelectionQuery(request.SalePointIds, request.Products, request.ProcurementId), cancellationToken);
            return Ok();
        }

        [Route("countByStatus")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductCountByStatuses(
            [FromBody, Required] GroupingFilterParametersRequest groupingFilterParametersRequest,
            CancellationToken cancellationToken)
        {
            var response =
                await Mediator.Send(new GetProductCountByStatusesQuery(groupingFilterParametersRequest),
                    cancellationToken);
            return Ok(response);
        }

        [Route("/api/v1/selections/products/change-quantity")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> ChangeQuantity([FromBody] ChangeProductsQuantity request, CancellationToken cancellationToken)
        {
            var selections = await Mediator.Send(new GetSelectionsBySalePointIdsQuery(new List<long>{request.SalePointId}), cancellationToken);
            if (!selections.Any())
                return NotFound($"selection with sale point id {request.SalePointId} is not found");

            foreach (var selection in selections)
            {
                var colorModelMeta = selection.ColorModelMetas.FirstOrDefault(cm =>
                    request.ColorModelVendorCodeSbs == cm.ColorModelVendorCodeSbs);

                if (colorModelMeta == null)
                    continue;

                await Mediator.Send(
                    new ChangeProductQuantityQuery(request.SizeChartCount, request.SizeInfos, colorModelMeta.Id, request.SizeChartId),
                    cancellationToken);
            }
            return Ok();
        }

        [Route("/api/v1/selections/{selectionId}/products/vendorCode/{modelVendorCodeSbs}")]
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveProductFromSelection([FromRoute, Required] long selectionId,
            [FromRoute, Required(AllowEmptyStrings = false)]
            string modelVendorCodeSbs, CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(selectionId), cancellationToken);

            if (selection == null)
                return NotFound($"selection with id {selectionId} is not found");

            selection.RemoveProduct(modelVendorCodeSbs);
            await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok();
        }

        [Route("/api/v1/selections/{selectionId}/products/colorModel")]
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveColorModelFromSelection([FromRoute, Required] long selectionId,
            [FromQuery] RemoveColorModelFromSelectionRequest request, CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(selectionId), cancellationToken);

            if (selection == null)
                return NotFound($"selection with id {selectionId} is not found");

            var filteredCmId = selection.ColorModelMetas.FirstOrDefault(cm =>
                cm.ColorModelGroupKeys.SalePointId == request.SalePointId &&
                cm.ColorModelVendorCodeSbs == request.ColorModelVendorCodeSbs)?.ColorModelVendorCodeSbs;

            if (filteredCmId == default)
                return NotFound(
                    $"colorModelMeta with DepartmentId {request.SalePointId} and SelectionId {selectionId} and ColorModelVendorCodeSbs {request.ColorModelVendorCodeSbs} is not found");

            selection.RemoveColorModel(filteredCmId);
            await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok();
        }

        [Route("by-sbs")]
        [HttpPost]
        [ProducesResponseType(typeof(List<ProductData>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetBySBS([FromBody] GetProductsBySBSRequest request, CancellationToken cancellationToken)
        {
            var products = await Mediator.Send(
                new GetProductsBySBSQuery(request.ModelVendoreCodeSbs, request.SelectionId, request.ProcurementId,
                    request.SalePointId, request.PurchaseKeyId),
                cancellationToken);
            
            return Ok(products);
        }
        
        
    }
}