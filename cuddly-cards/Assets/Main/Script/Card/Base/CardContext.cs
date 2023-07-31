using System;
using UnityEngine;
using static CardManager;
using static CardInfo;

public class CardContext
{

    public CardContext(string label, string description, CardType cardType)
    {
        Label = label;
        Description = description;
        CardType = cardType;
    }

    public CardContext(int id, string label, string description, CardType cardType) : this(label, description, cardType)
    {
        ID = id;
    }

    public int ID { get; private set; } = -1;
    public string Label { get; private set; }
    public string Description { get; private set; }
    public CardType CardType { get; private set; }
    public string DesiredKey { get; set; }
    public int TalkID { get; set; } = -1;
}