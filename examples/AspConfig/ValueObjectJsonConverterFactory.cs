using System.Text.Json;
using System.Text.Json.Serialization;
using ValueObject.Core;

namespace AspConfig;

/// <summary>
/// A JsonConverterFactory that can handle any IValueObject by serializing/deserializing the underlying primitive value.
/// It will be a part of a library later, but for now it's here to demonstrate how to create a custom JsonConverterFactory for value objects.
/// </summary>
public class ValueObjectJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return GetValueObjectInterface(typeToConvert) is not null;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var voInterface = GetValueObjectInterface(typeToConvert)!;
        var primitiveType = voInterface.GetGenericArguments()[0];

        var converterType = typeof(ValueObjectJsonConverter<,>)
            .MakeGenericType(typeToConvert, primitiveType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetValueObjectInterface(Type type) =>
        type
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IValueObject<>));

    private class ValueObjectJsonConverter<TValueObject, TPrimitive>
        : JsonConverter<TValueObject>
        where TValueObject : IValueObject<TPrimitive>
        where TPrimitive : notnull
    {
        private readonly Func<TPrimitive, TValueObject> _ctor;

        public ValueObjectJsonConverter()
        {
            var ctor = typeof(TValueObject).GetConstructor([typeof(TPrimitive)])
                       ?? throw new InvalidOperationException(
                           $"Type {typeof(TValueObject)} must have a constructor {typeof(TValueObject).Name}({typeof(TPrimitive).Name} value).");
            _ctor = value => (TValueObject)ctor.Invoke([value]);
        }

        public override TValueObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var primitive = JsonSerializer.Deserialize<TPrimitive>(ref reader, options);

            return primitive is null 
                ? throw new JsonException($"Cannot deserialize null into {typeof(TValueObject).Name}.")
                : _ctor(primitive);
        }

        public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}