using Microsoft.AspNetCore.Http;
using Superbrands.Libs.DDD.EfCore;
using Superbrands.Selection.Infrastructure.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Infrastructure
{
    internal class ColorModelMetaRepository : RepositoryBase<ColorModelMetaDalDto>, IColorModelMetaRepository
    {
        private readonly SelectionDbContext _context;

        public ColorModelMetaRepository(SelectionDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,
            httpContextAccessor)
        {}

    }
}
