using static CardManager;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static CardInfo;

public class SEObjectElement
{
    public string Label { get; set; }
    public string Description { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public CardType Type { get; set; }
    public int Depth { get; set; }

    public string DesiredKey { get; set; }
    public int TalkID { get; set; }
    public List<DialogueContext> Dialogue { get; set; }
    
}