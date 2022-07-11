using System;
using System.Threading;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Superbrands.Libs.Bus.Client;

namespace Superbrands.Selection.Bus
{
    public static class Dependencies
    {
        public static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
        {
            var assembliesToScan = typeof(Dependencies).Assembly;
            services.AddMediatR(assembliesToScan);
            services.AddBusAndAutoSubscription(configuration, assembliesToScan);
            BusMappings.Configure();
            return services;
        }

        public static IApplicationBuilder InitBus(this IApplicationBuilder applicationBuilder, IServiceProvider provider)
        {
            applicationBuilder.InitAutoSubscription(provider);
            return applicationBuilder;
        }
    }

}
