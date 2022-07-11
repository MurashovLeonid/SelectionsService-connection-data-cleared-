using Superbrands.Libs.DDD.EfCore.Abstractions;
using Superbrands.Selection.Infrastructure.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Infrastructure.Abstractions
{
    public interface IColorModelMetaRepository : IRepository<ColorModelMetaDalDto>
    {
    }
}
