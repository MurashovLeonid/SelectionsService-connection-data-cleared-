using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using JetBrains.Annotations;

namespace Superbrands.Selection.WebApi.Controllers
{
        [ApiController]
        public abstract class ControllerBase : Libs.Authentication.NetCore.AuthenticationControllerBase
    {
            protected readonly IMediator Mediator;

            protected ControllerBase([NotNull] IMediator mediator)
            {
                Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }
    }
}