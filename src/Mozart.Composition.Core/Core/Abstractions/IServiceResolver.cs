namespace Mozart.Composition.Core.Abstractions
{
    public interface IServiceResolver<in TKey, TService>
    {
        bool TryResolve(TKey key, out TService service);
    }
}