using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using NoMoreTears.Shared.CQS.Primitives.Attributes;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Serializes request and response payloads for logging with sensitive values masked.
/// </summary>
/// <remarks>
///     Masking is applied to every nested type, so a sensitive property cannot leak
///     through a wrapper object or a collection.
/// </remarks>
internal static class LogPayloadSerializer
{
    public const string Mask = "***";

    /// <summary>
    ///     Name fragments that mark a property as sensitive even without <see cref="SensitiveDataAttribute"/>.
    /// </summary>
    private static readonly string[] SensitiveNameFragments =
    [
        "Password",
        "Token",
        "Secret",
        "ApiKey",
        "Credential"
    ];

    /// <summary>
    ///     Property types the name-based rule applies to.
    /// </summary>
    private static readonly HashSet<Type> TextualTypes =
    [
        typeof(string),
        typeof(char[]),
        typeof(byte[]),
        typeof(ReadOnlyMemory<char>),
        typeof(ReadOnlyMemory<byte>)
    ];

    /// <summary>
    ///     Shared across all closed behavior types, so the type metadata cache is built once per payload type.
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { MaskSensitiveProperties }
        }
    };

    public static string Serialize<TPayload>(TPayload payload) =>
        JsonSerializer.Serialize(payload, SerializerOptions);

    private static void MaskSensitiveProperties(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var property in typeInfo.Properties)
        {
            if (IsSensitive(property))
            {
                property.CustomConverter = CreateMaskingConverter(property.PropertyType);
            }
        }
    }

    private static bool IsSensitive(JsonPropertyInfo property)
    {
        if (property.AttributeProvider?.IsDefined(typeof(SensitiveDataAttribute), inherit: true) == true)
        {
            return true;
        }

        if (!TextualTypes.Contains(property.PropertyType))
        {
            return false;
        }

        var name = (property.AttributeProvider as MemberInfo)?.Name ?? property.Name;

        return SensitiveNameFragments.Any(fragment => name.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }

    private static JsonConverter CreateMaskingConverter(Type propertyType) =>
        (JsonConverter)Activator.CreateInstance(typeof(MaskingConverter<>).MakeGenericType(propertyType))!;

    /// <summary>
    ///     Writes the mask instead of the value; null values are still written as null by the serializer.
    /// </summary>
    private sealed class MaskingConverter<T> : JsonConverter<T>
    {
        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            throw new NotSupportedException("Masked log payloads cannot be deserialized.");

        public override void Write(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(Mask);
    }
}