using Microsoft.EntityFrameworkCore.ChangeTracking;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Abstractions;

namespace Superbrands.Selection.Infrastructure
{
    public static class ComparsionHelper
    {
        public static ValueComparer<T> GetValueObjectComparer<T>() where T: ValueObject, IDeepCloneable<T>
        {
            return new(
                (l, r) => l.Equals(r),
                v => v == null ? 0 : v.GetHashCode(),
                v => v.DeepClone() // без этого не работает change tracking
            );
        }
    }
}