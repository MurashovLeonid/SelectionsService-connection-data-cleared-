using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Superbrands.Libs.DDD.EfCore.DbContext;
using Superbrands.Selection.Infrastructure.Configurations;
using Superbrands.Selection.Infrastructure.DAL;

namespace Superbrands.Selection.Infrastructure
{
    internal sealed class SelectionDbContext : SuperbrandsDbContext<SelectionDbContext>
    {
        public SelectionDbContext(DbContextOptions<SelectionDbContext> options, IMediator mediator,
            ILoggerFactory loggerFactory, IServiceProvider sp) : base(options, mediator, loggerFactory, sp)
        {
        }

        public DbSet<SelectionDalDto> Selections { get; set; }
        public DbSet<LogEntryDalDto> Logs { get; set; }
        public DbSet<SelectionPurchaseSalePointKeyDalDto> SelectionPurchaseSalePointKeys { get; set; }
        public DbSet<ColorModelMetaDalDto> ColorModelMetas { get; set; }
        public DbSet<ProcurementDalDto> Procurements { get; set; }
        public DbSet<NomenclatureTemplateDalDto> NomenclatureTemplates { get; set; }
        public DbSet<ProcurementKeySetDalDto> ProcurementKeySets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseHiLo();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ColorModelMetasConfiguration());
        }
    }
}