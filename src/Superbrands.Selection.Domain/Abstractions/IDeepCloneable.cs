namespace Superbrands.Selection.Domain.Abstractions
{
    public interface IDeepCloneable<T>
    {
        T DeepClone();
    }
}