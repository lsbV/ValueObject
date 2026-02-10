using ValueObject.Core;

namespace ValueObjectTests;

public class OperatorsTests
{
    private const string Name = "Victor Shnaider";

    [Fact]
    public void ImplicitOperator_Should_Convert_ValueObject_To_Primary_Type()
    {
        var name = new Name(Name);

        string value = name;

        Assert.Equal(name, value);
    }

    [Fact]
    public void ExplicitOperator_Should_Convert_Primary_Type_To_ValueObject()
    {
        string value = Name;

        Name name = (Name)value;

        Assert.Equal(value, name);
    }

    [Fact]
    public void GreaterOperator_Should_Work()
    {
        var length1 = new Length(123.5);
        var length2 = new Length(100);
        Assert.True(length1 > length2);
        Assert.True(length1 >= length2);
        Assert.False(length1 < length2);
        Assert.False(length1 <= length2);
        Assert.True(length1 > 100);
    }

    [Fact]
    public void AsExtension_Should_Convert_Primary_Type_To_AsWrapper()
    {
        string value = Name;

        var valueObject = value.As.Name;

        Assert.Equal(value, valueObject);
    }

}


public partial record Name(string Value) : IValueObject<string>;

public partial record Length(double Value) : IValueObject<double>;