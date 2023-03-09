using System.Text.Json;
using System.Text.Json.Serialization;

static class JsonDefault
{
    public static JsonSerializerOptions Value {get; }

    static JsonDefault()
        => Value = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
}