using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using Superbrands.Libs.HttpClient;
using Superbrands.Libs.RestClients.Selection;

namespace Superbrands.Selection.WebApi.Client
{
    public static class SelectionDependencies
    {
        public static void AddSelectionsClient(this IServiceCollection services, IConfiguration configuration)
        {
            var selectionsUri = configuration.GetSection("Selection")["BaseUrl"];

            if (string.IsNullOrEmpty(selectionsUri)) throw new Exception("Selection[BaseUrl] not found in appconfig");
            var baseUrl = new Uri(selectionsUri);
            
            services.AddTransient<ISelectionLogsClient, SelectionLogsClient>(x => new SelectionLogsClient(GetClient
                (x, baseUrl)));

            services.AddTransient<ISelectionProcurementsClient, SelectionProcurementsClient>(x =>
                new SelectionProcurementsClient(GetClient(x, baseUrl)));

            services.AddTransient<ISelectionProductsClient, SelectionProductsClient>(x =>
                new SelectionProductsClient(GetClient(x, baseUrl)));

            services.AddTransient<ISelectionSelectionsClient, SelectionSelectionsClient>(x =>
                new SelectionSelectionsClient(GetClient(x, baseUrl)));
        }

        private static SuperbrandsRestClient GetClient(IServiceProvider x, Uri baseUrl)
        {
            return new(x.GetRequiredService<IHttpContextAccessor>(), baseUrl);
        }
    }
}