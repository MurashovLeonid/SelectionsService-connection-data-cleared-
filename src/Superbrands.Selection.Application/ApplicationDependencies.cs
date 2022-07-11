using System;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Superbrands.ContextualSearch.WebApi.Client;
using Superbrands.Libs.RestClients.FileStorage;
using Superbrands.Libs.Restclients.Members;
using Superbrands.Libs.RestClients.Partners;
using Superbrands.Libs.RestClients.PIM;
using Superbrands.Libs.RestClients.Seasons;
using Superbrands.Libs.RestClients.SSO;
using Superbrands.PartnerInfrastructure.WebApi.Client;
using Superbrands.Selection.Application.Options;

namespace Superbrands.Selection.Application
{
    public static class ApplicationDependencies
    {
        public static void RegisterApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembliesToScan = typeof(ApplicationDependencies).Assembly;
            services.AddMediatR(assembliesToScan);
            services.Configure<ExportOptions>(configuration.GetSection("ExportOptions"));

            services.AddPartnerInfrastructureClients(configuration);
            services.AddMembersClient(configuration);

            services.RegisterPartnersClient(configuration);
            services.AddSeasonsClients(configuration);
            services.RegisterSSOClient(configuration);
            PimClientDependencies.Register(services, configuration);
            FileStorageClientDependencies.Register(services, configuration);

            services.AddSuperbrandsContextualSearchClient(new Uri(configuration.GetConnectionString("ContextualSearchUri")));
            AppMappings.Configure();
        }
    }
}