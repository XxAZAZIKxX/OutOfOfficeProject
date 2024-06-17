using System.Text.Json.Serialization;
using OutOfOffice.Core.Converters;

namespace OutOfOffice.Core.Utilities;

[JsonConverter(typeof(OptionalJsonConverterFactory))]
public readonly struct Optional<T>
{
    private readonly T? _value;
    public bool HasValue { get; } = false;
    public T Value => _value ?? throw new InvalidOperationException("No value present");
    public static Optional<T> None => new();

    public Optional(T value)
    {
        _value = value;
        HasValue = true;
    }

    public static implicit operator Optional<T>(T value) => new(value);

    public override bool Equals(object? obj)
    {
        if (obj is not Optional<T> other) return false;

        return _value?.Equals(obj) ?? EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);
    public static bool operator !=(Optional<T> left, Optional<T> right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(HasValue, _value);

    public override string ToString()
    {
        return HasValue ? _value!.ToString()! : "No value";
    }
}