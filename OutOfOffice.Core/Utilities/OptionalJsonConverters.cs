using System.Text.Json.Serialization;
using System.Text.Json;

namespace OutOfOffice.Core.Utilities;


public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Optional<T>.None;
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return new Optional<T>(value!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (value.HasValue) JsonSerializer.Serialize(writer, value.Value, options);
        else writer.WriteNullValue();
    }
}

public class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var genericArgument = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalJsonConverter<>).MakeGenericType(genericArgument);

        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}