using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace POrpg.Networking;

public class PolymorphicConverterFactory<TBase> : JsonConverterFactory
{
    private readonly Dictionary<string,Type> _typeByDiscriminator;
    private readonly Dictionary<Type,string> _discriminatorByType;

    public PolymorphicConverterFactory()
    {
        var types = typeof(TBase).Assembly.GetTypes()
            .Where(t => typeof(TBase).IsAssignableFrom(t) && !t.IsAbstract);

        _typeByDiscriminator = types.ToDictionary(
            t => t.Name,
            t => t);

        _discriminatorByType = types.ToDictionary(
            t => t,
            t => t.Name);
    }

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert == typeof(TBase);

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions opts)
        => (JsonConverter)Activator.CreateInstance(
            typeof(PolymorphicConverter<>).MakeGenericType(type),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [_typeByDiscriminator, _discriminatorByType],
            culture: null
        )!;
}

public class PolymorphicConverter<TBase> : JsonConverter<TBase>
{
    private readonly Dictionary<string,Type>  _typeByDisc;
    private readonly Dictionary<Type,string>  _discByType;

    public PolymorphicConverter(
        Dictionary<string,Type>  typeByDisc,
        Dictionary<Type,string>  discByType)
    {
        _typeByDisc = typeByDisc;
        _discByType = discByType;
    }

    public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var discProp))
            throw new JsonException("Missing discriminator");

        var disc = discProp.GetString()!;
        if (!_typeByDisc.TryGetValue(disc, out var concrete))
            throw new JsonException($"Unknown type discriminator '{disc}'");

        return (TBase?)JsonSerializer.Deserialize(root.GetRawText(), concrete, options);
    }

    public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions opts)
    {
        var actualType = value.GetType();
        if (!_discByType.TryGetValue(actualType, out var disc))
            throw new JsonException($"Type {actualType} not registered for polymorphism");

        writer.WriteStartObject();
        writer.WriteString("type", disc);

        foreach (var prop in actualType.GetProperties(BindingFlags.Public|BindingFlags.Instance))
        {
            if (prop.CanRead && prop.GetMethod!.GetParameters().Length == 0)
            {
                writer.WritePropertyName(opts.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name);
                JsonSerializer.Serialize(writer, prop.GetValue(value), prop.PropertyType, opts);
            }
        }
        writer.WriteEndObject();
    }
}
