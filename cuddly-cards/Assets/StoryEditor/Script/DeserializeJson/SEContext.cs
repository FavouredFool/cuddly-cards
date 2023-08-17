using static CardManager;
using static CardInfo;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class SEContext
{
    public int CardID;

    public CardType CardType;

    public string Label;

    public string Description;

    public int DesiredKeyID;

    public int DesiredTalkID;

    public List<DialogueContext> DialogueContext;

    public int Depth;

    public SEContext(string label, string description, CardType cardType)
    {
        Label = label;
        Description = description;
        CardType = cardType;
    }

    public SEContext(int depth, string label, string description, CardType cardType)
    {
        Depth = depth;
        Label = label;
        Description = description;
        CardType = cardType;
    }
}