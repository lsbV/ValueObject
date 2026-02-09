using ValueObject.Core;

namespace ValueObjectTests;

public class ConverterTests
{
    [Fact]
    public void ValueConverter_Should_Convert_ValueObject_To_Primary_Type()
    {
        var age = new Age(30);
        var converter = new AgeValueConverter();
        var converted = converter.ConvertToProviderExpression.Compile().Invoke(age);
        Assert.Equal(age.Value, converted);
    }

    [Fact]
    public void ValueConverter_Should_Convert_Primary_Type_To_ValueObject()
    {
        int value = 30;
        var converter = new AgeValueConverter();
        var converted = converter.ConvertFromProviderExpression.Compile().Invoke(value);
        Assert.Equal(value, converted.Value);
    }

    [Fact]
    public void ValueConverter_Should_Handle_Null_ValueObject()
    {
        Age? age = null;
        var converter = new AgeNullableValueConverter();
        var converted = converter.ConvertToProviderExpression.Compile().Invoke(age);
        Assert.Null(converted);
    }
}

public partial record Age(int Value) : IValueObject<int>;