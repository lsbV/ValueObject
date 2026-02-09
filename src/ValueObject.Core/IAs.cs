namespace ValueObject.Core;

public interface IAs<T>
{
    abstract T Value { get; }
}