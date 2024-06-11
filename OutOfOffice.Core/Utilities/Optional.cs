namespace OutOfOffice.Core.Utilities;

public struct Optional
{
    public static Optional None = new();
}

public readonly struct Optional<T>
{
    private readonly T? _value;
    public bool HasValue { get; private init; } = false;
    public T Value => _value ?? throw new InvalidOperationException("No value present");
    public static Optional<T> None => new();


    public Optional(T value)
    {
        _value = value;
        HasValue = true;
    }

    public static implicit operator Optional<T>(T value) => new(value);
    public static implicit operator Optional<T>(Optional _) => None;

    public override bool Equals(object? obj)
    {
        if (obj is not Optional<T> other) return false;

        return _value!.Equals(obj) ||
            EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);
    public static bool operator !=(Optional<T> left, Optional<T> right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(HasValue, _value);

    public override string ToString()
    {
        return HasValue ? _value!.ToString()! : "No value";
    }
}