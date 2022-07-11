using System.Runtime.CompilerServices;
using Mapster;
using Superbrands.Selection.Domain;
using Superbrands.Selection.Domain.Selections;

[assembly: InternalsVisibleTo("Superbrands.Selection.UnitTests")]

namespace Superbrands.Selection.Application
{
    internal class AppMappings
    {
        public static void Configure()
        {
            TypeAdapterConfig<Size, Size>.NewConfig().ConstructUsing(x => x);
        }
    }
}