using System;

namespace ValueObject.Core;

[AttributeUsage(AttributeTargets.Assembly)]
public class ValueObjectSettingsAttribute : Attribute
{
    public bool GenerateMongoDbSerializer { get; set; } = true;
    public bool GenerateEfCoreValueConverter { get; set; } = true;

    public ValueObjectSettingsAttribute(bool generateMongoDbSerializer,
                                        bool generateEfCoreValueConverter)
    {
        GenerateMongoDbSerializer = generateMongoDbSerializer;
        GenerateEfCoreValueConverter = generateEfCoreValueConverter;
    }
}