using static CardManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static CardInfo;

public class DeserializedObjectElement
{
    public string Label { get; set; }
    public string Description { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public CardType Type { get; set; }
    public int Depth { get; set; }

    public string DesiredKey { get; set; }
}