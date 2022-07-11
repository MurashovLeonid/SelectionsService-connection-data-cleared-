using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Superbrands.Libs.Authentication.NetCore;
using Superbrands.Selection.Application.Logs;
using Superbrands.Selection.WebApi.DTOs;

namespace Superbrands.Selection.WebApi.Controllers
{
    [Route("api/v1/selections/")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class LogsController : AuthenticationControllerBase
    {
        private readonly IMediator _mediator;

        public LogsController([NotNull] IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [Route("{selectionId}/logs")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LogEntryDto>), 200)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSelectionLogs(long selectionId, CancellationToken cancellationToken)
        {
            if (selectionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(selectionId), "selection id cannot be less than zero");

            var logs = await _mediator.Send(new GetLogsBySelectionId(selectionId), cancellationToken);
            return Ok(logs.Select(x=> new LogEntryDto(x)).ToList());
        }
    }
}