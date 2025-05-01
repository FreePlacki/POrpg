using System.Text.Json;
using System.Text.Json.Serialization;

namespace POrpg.Networking;

public class TwoDimensionalArrayJsonConverter<T> : JsonConverter<T[,]>
{
    public override T[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        // Peek at the first row to determine the column count
        var temp = JsonDocument.ParseValue(ref reader);

        int rowLength = temp.RootElement.GetArrayLength();
        if (rowLength == 0)
            return new T[0, 0];

        int columnLength = temp.RootElement[0].GetArrayLength();

        var result = new T[rowLength, columnLength];

        int i = 0;
        foreach (var row in temp.RootElement.EnumerateArray())
        {
            int j = 0;
            foreach (var col in row.EnumerateArray())
            {
                // Deserialize each cell using options!
                result[i, j] = col.Deserialize<T>(options)!;
                j++;
            }
            i++;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        for (int i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();
            for (int j = 0; j < value.GetLength(1); j++)
            {
                JsonSerializer.Serialize(writer, value[i, j], options);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}