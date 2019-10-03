namespace Vavatech.DotnetCore.Models
{

    public interface IEntity<TKey>
    {
        TKey Id { get; set;  }
    }

    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
