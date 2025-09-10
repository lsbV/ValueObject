using ValueObject.Core;

namespace ValueObjectTests;

public class OperatorsTests
{
    private const string NAME = "Victor Shnaider";

    [Fact]
    public void ImplisitOperator_Should_Convert_ValueObject_To_Primiry_Type()
    {
        var name = new Name(NAME);

        string value = name;

        Assert.Equal(name, value);
    }

    [Fact]
    public void ExplicitOperator_Should_Convert_Primiry_Type_To_ValueObject()
    {
        string value = NAME;

        Name name = (Name)value;

        Assert.Equal(value, name);
    }

    [Fact]
    public void AsExtension_Should_Convert_Primiry_Type_To_AsWrapper()
    {
        string value = NAME;

        var valueObject = value.As.Name;

        Assert.Equal(value, valueObject);
    }

}


public partial record Name(string Value) : IValueObject<string>;
