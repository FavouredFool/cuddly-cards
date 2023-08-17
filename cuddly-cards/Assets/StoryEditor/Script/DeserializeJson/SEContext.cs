using static CardManager;
using static CardInfo;
using System;

[Serializable]
public class SEContext
{
    public CardType CardType;
    public string Label;
    public string Description;
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