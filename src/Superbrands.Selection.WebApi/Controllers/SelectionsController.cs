using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Superbrands.Selection.Application.Requests;
using Superbrands.Selection.Application.Selection;
using Superbrands.Selection.Application.Templates;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Bus.Contracts.CSharp.SSO;
using Superbrands.Libs.Authentication.NetCore;
using Superbrands.Selection.Application.Procurement;
using SelectionStatus = Superbrands.Selection.Domain.Enums.SelectionStatus;

namespace Superbrands.Selection.WebApi.Controllers
{
    [Route("api/v1/selections")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SelectionsController : ControllerBase
    {
        private readonly ISelectionRepository _selectionRepository;

        public SelectionsController(IMediator mediator, ISelectionRepository selectionRepository)
            : base(mediator)
        {
            _selectionRepository = selectionRepository ?? throw new ArgumentNullException(nameof(selectionRepository));
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<SelectionDto>), 200)]
        [SuperbrandsAuthorize("Selections", OperationType.Read)]
        public async Task<IActionResult> GetAvailableSelections(CancellationToken cancellationToken)
        {
            var selections = await Mediator.Send(new GetUserSelectionsQuery(GetCurrentUserIdLong().Value),
                cancellationToken);
            return Ok(selections.Select(x => new SelectionDto(x)));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SelectionDto), 200)]
        public async Task<IActionResult> GetSelectionById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(id), cancellationToken);
            return Ok(new SelectionDto(selection));
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SelectionDto>), 200)]
        public async Task<IActionResult> GetSelectionsByIds([FromQuery, Required] List<long> ids, CancellationToken cancellationToken)
        {
            var selections = await Mediator.Send(new GetSelectionsByIdsQuery(ids), cancellationToken);
            var dtos = selections.Select(s => new SelectionDto(s)).ToList();
            return Ok(dtos);
        }
        
        [HttpGet("by-procurement")]
        [ProducesResponseType(typeof(IEnumerable<SelectionDto>), 200)]
        public async Task<IActionResult> GetSelectionsByProcurementId([FromQuery, Required] long procurementId, CancellationToken cancellationToken)
        {
            var selections = await Mediator.Send(new GetSelectionsByProcurementIdQuery(procurementId), cancellationToken);
            var dtos = selections.Select(s => new SelectionDto(s)).ToList();
            return Ok(dtos);
        }


        [HttpGet("{id}/export")]
        [ProducesResponseType(typeof(ProductsResponse), 200)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> ExportSelection([FromRoute] long id, CancellationToken cancellationToken)
        {
            return await Mediator.Send(new ExportSelectionQuery(id), cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SelectionDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [SuperbrandsAuthorize("Selections", OperationType.Create)]
        public async Task<IActionResult> CreateSelection(CreateSelectionRequest request,
            CancellationToken cancellationToken)
        {
            var selection = new Domain.Selections.Selection( GetCurrentUserIdLong().Value, request.ProcurementId, SelectionStatus.SelectionIsEmpty);
            var createdSelection = await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok(new SelectionDto(createdSelection));
        }

        [HttpPut("changeSizesAndSizeChart")]
        public async Task<IActionResult> ChangeSizesWithSizeChart(
            [FromBody] ChangeSizesWithSizeChartRequest changeSizesWithSizeChartQuery)
        {
            await Mediator.Send(new ChangeSizesWithSizeChartQuery(changeSizesWithSizeChartQuery.ColorModelVendorCodeSbs,
                changeSizesWithSizeChartQuery.SizeChartId,
                changeSizesWithSizeChartQuery.SizeChartCount, changeSizesWithSizeChartQuery.SizesSkuAndCount,
                changeSizesWithSizeChartQuery.SalePointIds));

            return Ok();
        }

        [HttpPost("setStatus")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetStatus([FromQuery, Required] long selectionId,
            [FromQuery, Required] SelectionStatus newStatus,
            CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(selectionId), cancellationToken);

            if (selection == null)
                return NotFound();

            try
            {
                selection.SetStatus(newStatus);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(id), cancellationToken);

            if (selection == null)
                return NotFound();

            var currentUserId = GetCurrentUserIdLong();
            if (selection.BuyerId != currentUserId)
                return Unauthorized($"Client {GetCurrentUser()} does not have a selection with id {id}");
            
            var procurement = await Mediator.Send(new GetProcurementByIdQuery(selection.ProcurementId), cancellationToken);
            procurement.DeleteSelection(selection.Id);
            
            await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok();
        }

        [HttpPost("{id}/clear")]
        [ProducesResponseType(typeof(long), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> ClearSelection([FromRoute] int id, CancellationToken cancellationToken)
        {
            var selection = await Mediator.Send(new GetSelectionByIdQuery(id), cancellationToken);

            if (selection == null)
                return NotFound();

            selection.ClearProducts();

            await Mediator.Send(new UpsertSelectionQuery(selection), cancellationToken);
            return Ok();
        }

        [ProducesResponseType(typeof(NomenclatureTemplateDto), (int) HttpStatusCode.OK)]
        [HttpGet("templates/{id}")]
        public async Task<IActionResult> GetNomenclatureTemplateById([FromRoute] int id,
            CancellationToken cancellationToken)
        {
            var template = await Mediator.Send(new GetTemplateByIdQuery(id), cancellationToken);
            var dto = new NomenclatureTemplateDto(template);
            return Ok(dto);
        }

        [ProducesResponseType(typeof(IEnumerable<NomenclatureTemplateDto>), (int) HttpStatusCode.OK)]
        [HttpGet("templates")]
        public async Task<IActionResult> GetAllNomenclatureTemplates(CancellationToken cancellationToken)
        {
            var templates = await Mediator.Send(new GetAllTemplatesQuery(), cancellationToken);
            var dtos = templates.Select(t => new NomenclatureTemplateDto(t));
            return Ok(dtos);
        }

        [ProducesResponseType(typeof(NomenclatureTemplateDto), (int) HttpStatusCode.OK)]
        [HttpPost("templates")]
        public async Task<IActionResult> CreateNomenclatureTemlate(NomenclatureTemplateDto nomenclatureTemplateDto,
            CancellationToken cancellationToken)
        {
            var nomenclatureTemplate = nomenclatureTemplateDto.ToDomain();
            var templates = await Mediator.Send(new CreateTemplateQuery(nomenclatureTemplate), cancellationToken);
            return Ok(templates);
        }
    }
}