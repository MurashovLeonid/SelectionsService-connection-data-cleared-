using System;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Superbrands.Libs.MicroservicesFramework;
using Superbrands.Selection.Application;
using Superbrands.Selection.Bus;
using Superbrands.Selection.Infrastructure;

namespace Superbrands.Selection.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json")
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSuperbrandsDefaults(Configuration, "Selection");
            services.RegisterDal(Configuration);
            services.RegisterApplication(Configuration);

            AddMapper(services);
            services.AddBus(Configuration);
        }

        private static void AddMapper(IServiceCollection services)
        {
            services.AddTransient<IMapper, Mapper>();
            var config = TypeAdapterConfig.GlobalSettings;
            services.AddSingleton(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            app.UseSuperbrandsDefaults(env, "Selection");
            app.UseMvc();
            app.InitBus(provider);
        }
    }
}