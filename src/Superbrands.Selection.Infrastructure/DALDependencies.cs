using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Superbrands.Libs.DDD.EfCore.DbContext;
using Superbrands.Selection.Infrastructure.Abstractions;

namespace Superbrands.Selection.Infrastructure
{
    public static class DALDependencies
    {
        public static void RegisterDal(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("postgres");

            if (string.IsNullOrEmpty(connectionString))
                throw new MissingFieldException("Connection string with name 'postgres' is required");

            services.AddScoped<EntityModificationInfoInterceptor>();
            services.AddDbContext<SelectionDbContext>(builder =>
            {
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
                builder.UseNpgsql(new NpgsqlConnection(connectionString));
            });

            services.AddScoped<IProductMetaRepository, ProductMetaRepository>();
            services.AddScoped<IProcurementRepository, ProcurementRepository>();
            services.AddScoped<ISelectionRepository, SelectionRepository>();
            services.AddScoped<IColorModelMetaRepository, ColorModelMetaRepository>();
            DalMappings.Configure();
        }
    }
}