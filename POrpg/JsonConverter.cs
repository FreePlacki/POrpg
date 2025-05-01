namespace POrpg;

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq;

public class TwoDimensionalIntArrayJsonConverter<T> : JsonConverter<T[,]>
{    
    public override T[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        
        var rowLength = jsonDoc.RootElement.GetArrayLength();
        var columnLength = jsonDoc.RootElement.EnumerateArray().First().GetArrayLength();

        var grid = new T[rowLength, columnLength];

        var row = 0;
        foreach (var array in jsonDoc.RootElement.EnumerateArray())
        {
            var column = 0;
            foreach (var elem in array.EnumerateArray())
            {
                grid[row, column] = JsonSerializer.Deserialize<T>(elem)!;
                column++;
            }
            row++;
        }

        return grid;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        for (int i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();
            for (int j = 0; j < value.GetLength(1); j++)
            {
                writer.WriteStringValue(JsonSerializer.Serialize(value[i, j]));
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }   
}