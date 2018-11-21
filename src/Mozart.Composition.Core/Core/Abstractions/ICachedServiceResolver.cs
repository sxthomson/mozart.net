namespace Mozart.Composition.Core.Abstractions
{
    public interface ICachedServiceResolver<in TKey, TService>
    {
        bool TryResolve(TKey key, out TService service);
    }
}