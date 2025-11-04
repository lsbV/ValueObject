namespace ValueObject.Core;

public interface IValueObject<TValue>
{
    public abstract TValue Value { get; init; }
}
