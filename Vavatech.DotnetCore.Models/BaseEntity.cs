namespace Vavatech.DotnetCore.Models
{

    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }

    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; protected set; }
    }
}
