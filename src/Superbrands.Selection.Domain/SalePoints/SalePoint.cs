using System;
using System.Collections.Generic;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.Domain.SalePoints
{
    public class SalePoint : ValueObject
    {
        public long Id { get; }
        
        /// <summary>
        /// Участвует в закупке
        /// </summary>
        public bool IsActive { get; set; }

        public SalePoint(long id, bool isActive)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
            IsActive = isActive;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return IsActive;
        }
    }
}