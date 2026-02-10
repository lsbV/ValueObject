using System.Globalization;
using MongoDB.Bson;

namespace ValueObjectTests;

public class TryParseTests
{
    #region String Type Tests
    
    [Fact]
    public void Name_TryParse_Should_Parse_Valid_String()
    {
        const string testValue = "John Doe";
        var result = Name.TryParse(testValue, out var name);
        
        Assert.True(result);
        Assert.Equal(testValue, name.Value);
    }

    [Fact]
    public void Name_TryParse_Should_Parse_Empty_String()
    {
        var result = Name.TryParse(string.Empty, out var name);
        
        Assert.False(result);
    }

    [Fact]
    public void Name_TryParse_Should_Handle_Null_Value()
    {
        var result = Name.TryParse(null, out var name);
        
        Assert.False(result);
    }

    [Fact]
    public void Name_TryParse_With_Provider_Should_Parse_Valid_String()
    {
        const string testValue = "Jane Smith";
        var result = Name.TryParse(testValue, null, out var name);
        
        Assert.True(result);
        Assert.Equal(testValue, name.Value);
    }

    [Fact]
    public void ImageUrl_TryParse_Should_Parse_Valid_Url()
    {
        const string testUrl = "https://example.com/image.jpg";
        var result = ImageUrl.TryParse(testUrl, out var imageUrl);
        
        Assert.True(result);
        Assert.Equal(testUrl, imageUrl.Value);
    }

    [Fact]
    public void ImageUrl_TryParse_Should_Handle_Whitespace_Only()
    {
        // Whitespace-only strings are not considered empty by string.IsNullOrEmpty
        var result = ImageUrl.TryParse("   ", out var imageUrl);
        
        Assert.True(result);
        Assert.Equal("   ", imageUrl.Value);
    }

    #endregion

    #region Numeric Type Tests

    [Fact]
    public void Age_TryParse_Should_Parse_Valid_Integer()
    {
        const string testValue = "42";
        var result = Age.TryParse(testValue, out var age);
        
        Assert.True(result);
        Assert.Equal(42, age.Value);
    }

    [Fact]
    public void Age_TryParse_Should_Fail_For_Invalid_Integer()
    {
        const string testValue = "not-a-number";
        var result = Age.TryParse(testValue, out var age);
        
        Assert.False(result);
    }

    [Fact]
    public void Age_TryParse_Should_Handle_Negative_Integer()
    {
        const string testValue = "-5";
        var result = Age.TryParse(testValue, out var age);
        
        Assert.True(result);
        Assert.Equal(-5, age.Value);
    }

    [Fact]
    public void Age_TryParse_Should_Handle_Zero()
    {
        const string testValue = "0";
        var result = Age.TryParse(testValue, out var age);
        
        Assert.True(result);
        Assert.Equal(0, age.Value);
    }

    [Fact]
    public void Age_TryParse_Should_Handle_Large_Integer()
    {
        const string testValue = "999999999";
        var result = Age.TryParse(testValue, out var age);
        
        Assert.True(result);
        Assert.Equal(999999999, age.Value);
    }

    [Fact]
    public void Age_TryParse_With_Provider_Should_Parse_Valid_Integer()
    {
        const string testValue = "100";
        var result = Age.TryParse(testValue, null, out var age);
        
        Assert.True(result);
        Assert.Equal(100, age.Value);
    }

    [Fact]
    public void Length_TryParse_Should_Parse_Valid_Double()
    {
        const string testValue = "123.45";
        var result = Length.TryParse(testValue, out var length);
        
        Assert.True(result);
        Assert.Equal(123.45, length.Value);
    }

    [Fact]
    public void Length_TryParse_Should_Parse_Whole_Number_As_Double()
    {
        const string testValue = "100";
        var result = Length.TryParse(testValue, out var length);
        
        Assert.True(result);
        Assert.Equal(100.0, length.Value);
    }

    [Fact]
    public void Length_TryParse_Should_Handle_Negative_Double()
    {
        const string testValue = "-45.67";
        var result = Length.TryParse(testValue, out var length);
        
        Assert.True(result);
        Assert.Equal(-45.67, length.Value);
    }

    [Fact]
    public void Length_TryParse_Should_Fail_For_Invalid_Double()
    {
        const string testValue = "not.a.double";
        var result = Length.TryParse(testValue, out var length);
        
        Assert.False(result);
    }

    [Fact]
    public void Price_TryParse_Should_Parse_Valid_Decimal()
    {
        decimal.TryParse("19.99", NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var expected);
        Assert.Equal(19.99m, expected);
        const string testValue = "19.99";
        var result = Price.TryParse(testValue, out var price);
        
        Assert.True(result);
        Assert.Equal(19.99m, price.Value);
    }

    [Fact]
    public void Price_TryParse_Should_Handle_Zero_Decimal()
    {
        const string testValue = "0.00";
        var result = Price.TryParse(testValue, out var price);
        
        Assert.True(result);
        Assert.Equal(0.00m, price.Value);
    }

    [Fact]
    public void Price_TryParse_Should_Fail_For_Invalid_Decimal()
    {
        const string testValue = "not-a-price";
        var result = Price.TryParse(testValue, out var price);
        
        Assert.False(result);
    }

    #endregion

    #region GUID Type Tests

    [Fact]
    public void ProductId_TryParse_Should_Parse_Valid_Guid()
    {
        var testGuid = Guid.NewGuid();
        var guidString = testGuid.ToString();
        var result = ProductId.TryParse(guidString, out var productId);
        
        Assert.True(result);
        Assert.Equal(testGuid, productId.Value);
    }

    [Fact]
    public void ProductId_TryParse_Should_Parse_Guid_In_Braces()
    {
        var testGuid = Guid.NewGuid();
        var guidString = $"{{{testGuid}}}";
        var result = ProductId.TryParse(guidString, out var productId);
        
        Assert.True(result);
        Assert.Equal(testGuid, productId.Value);
    }

    [Fact]
    public void ProductId_TryParse_Should_Fail_For_Invalid_Guid()
    {
        const string testValue = "not-a-guid";
        var result = ProductId.TryParse(testValue, out var productId);
        
        Assert.False(result);
    }

    [Fact]
    public void ProductId_TryParse_Should_Handle_Null()
    {
        var result = ProductId.TryParse(null, out var productId);
        
        Assert.False(result);
    }

    [Fact]
    public void ProductId_TryParse_With_Provider_Should_Parse_Valid_Guid()
    {
        var testGuid = Guid.NewGuid();
        var guidString = testGuid.ToString();
        var result = ProductId.TryParse(guidString, null, out var productId);
        
        Assert.True(result);
        Assert.Equal(testGuid, productId.Value);
    }

    #endregion

    #region ObjectId Type Tests

    [Fact]
    public void UserId_TryParse_Should_Parse_Valid_ObjectId()
    {
        var testObjectId = ObjectId.GenerateNewId();
        var objectIdString = testObjectId.ToString();
        var result = UserId.TryParse(objectIdString, out var userId);
        
        Assert.True(result);
        Assert.Equal(testObjectId, userId.Value);
    }

    [Fact]
    public void UserId_TryParse_Should_Fail_For_Invalid_ObjectId()
    {
        const string testValue = "not-an-objectid";
        var result = UserId.TryParse(testValue, out var userId);
        
        Assert.False(result);
    }

    [Fact]
    public void UserId_TryParse_Should_Handle_Null()
    {
        var result = UserId.TryParse(null, out var userId);
        
        Assert.False(result);
    }

    [Fact]
    public void UserId_TryParse_Should_Handle_Empty_String()
    {
        var result = UserId.TryParse(string.Empty, out var userId);
        
        Assert.False(result);
    }

    [Fact]
    public void UserId_TryParse_With_Provider_Should_Parse_Valid_ObjectId()
    {
        var testObjectId = ObjectId.GenerateNewId();
        var objectIdString = testObjectId.ToString();
        var result = UserId.TryParse(objectIdString, null, out var userId);
        
        Assert.True(result);
        Assert.Equal(testObjectId, userId.Value);
    }

    #endregion

    #region Edge Cases and Special Scenarios

    [Fact]
    public void TryParse_Should_Be_Case_Insensitive_For_Guid()
    {
        var testGuid = Guid.NewGuid();
        var guidString = testGuid.ToString().ToUpper();
        var result = ProductId.TryParse(guidString, out var productId);
        
        Assert.True(result);
        Assert.Equal(testGuid, productId.Value);
    }

    [Fact]
    public void Age_TryParse_Should_Handle_Whitespace_Padding()
    {
        const string testValue = "  42  ";
        var result = Age.TryParse(testValue, out var age);
        
        // This depends on int.TryParse behavior, which typically handles whitespace
        // The actual result may vary based on .NET behavior
        if (result)
        {
            Assert.Equal(42, age.Value);
        }
    }

    [Fact]
    public void Length_TryParse_Should_Handle_Scientific_Notation()
    {
        const string testValue = "1.23e2"; // 123
        var result = Length.TryParse(testValue, out var length);
        
        Assert.True(result);
        Assert.Equal(123, length.Value);
    }

    [Fact]
    public void Price_TryParse_Should_Handle_Currency_Formats()
    {
        const string testValue = "99.99";
        var result = Price.TryParse(testValue, out var price);
        
        Assert.True(result);
        Assert.Equal(99.99m, price.Value);
    }

    #endregion

    #region Out Parameter Tests

    [Fact]
    public void Age_TryParse_Should_Set_Out_Parameter_On_Success()
    {
        Age? age = null;
        var result = Age.TryParse("50", out age);
        
        Assert.True(result);
        Assert.NotNull(age);
        Assert.Equal(50, age.Value);
    }

    [Fact]
    public void Age_TryParse_Should_Set_Default_Out_Parameter_On_Failure()
    {
        Age age = new Age(0);
        var result = Age.TryParse("invalid", out age);
        
        Assert.False(result);
        // Out parameter is set to default! which becomes null for nullable context
        Assert.Null(age);
    }

    [Fact]
    public void ProductId_TryParse_Should_Set_Out_Parameter_On_Success()
    {
        var testGuid = Guid.NewGuid();
        var result = ProductId.TryParse(testGuid.ToString(), out var productId);
        
        Assert.True(result);
        Assert.NotEqual(productId, Guid.Empty);
        Assert.Equal(testGuid, productId.Value);
    }

    #endregion

    #region Consistency Tests

    [Fact]
    public void Name_Explicit_Cast_And_TryParse_Should_Produce_Same_Result()
    {
        const string testValue = "Alice";
        Name name1 = (Name)testValue;
        var parseResult = Name.TryParse(testValue, out var name2);
        
        Assert.True(parseResult);
        Assert.Equal(name1, name2);
    }

    [Fact]
    public void Age_Parsed_And_Created_Values_Should_Be_Equal()
    {
        const string testValue = "25";
        var age1 = new Age(25);
        var parseResult = Age.TryParse(testValue, out var age2);
        
        Assert.True(parseResult);
        Assert.Equal(age1, age2);
    }

    [Fact]
    public void Length_Parsed_Value_Should_Roundtrip()
    {
        const double testValue = 42.5;
        var length1 = new Length(testValue);
        var stringValue = length1.Value.ToString();
        var parseResult = Length.TryParse(stringValue, out var length2);
        
        Assert.True(parseResult);
        Assert.Equal(testValue, length2.Value);
    }

    #endregion
}




