using Superbrands.Libs.DDD.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Superbrands.Selection.Domain
{
    public class NomenclatureTemplate : EntityBase
    {
        public string Name { get; protected set; }
        public IEnumerable<int> Parameters { get; protected set; }

        protected NomenclatureTemplate()
        {
        }

        public NomenclatureTemplate(string name, IEnumerable<int> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            if (!parameters.Any())
                throw new ArgumentException(nameof(parameters));

            Name = name;
            Parameters = parameters;
        }
    }
}
