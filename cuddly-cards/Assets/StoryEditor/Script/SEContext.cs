using System;
using UnityEngine;
using System.Collections.Generic;
using static CardManager;
using static CardInfo;

public class SEContext
{

    public SEContext(string label, string description, CardType cardType)
    {
        Label = label;
        Description = description;
        CardType = cardType;
    }

    public SEContext(int id, string label, string description, CardType cardType) : this(label, description, cardType)
    {
        ID = id;
    }

    public int ID { get; private set; } = -1;
    public string Label { get; private set; }
    public string Description { get; private set; }
    public CardType CardType { get; private set; }
    public string DesiredKey { get; set; }
    public int TalkID { get; set; } = -1;
    public List<DialogueContext> DialogueContexts { get; set; }
}