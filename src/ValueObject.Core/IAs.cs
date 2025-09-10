namespace ValueObject.Core;

public interface IAs<T>
{
    protected abstract T Value { get; }
}