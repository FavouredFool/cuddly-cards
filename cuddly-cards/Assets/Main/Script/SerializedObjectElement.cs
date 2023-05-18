using static CardManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class SerializedObjectElement
{
    public string Label { get; set; }
    public string Description { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public CardType Type { get; set; }
    public int Depth { get; set; }
}