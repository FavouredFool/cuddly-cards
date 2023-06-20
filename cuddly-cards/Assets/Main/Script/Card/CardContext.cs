using static CardManager;
using static CardInfo;
using UnityEngine;

public class CardContext
{
    string _label;
    string _description;
    CardType _cardType;

    // color is derived from cardtype but also needed to change the renderCamera background -> needs to be added manually
    Color _backgroundColor;

    bool _hasBeenSeen;

    string _modelName;


    public CardContext(string label, string description, CardType cardType)
    {
        _label = label;
        _description = description;
        _cardType = cardType;

        _hasBeenSeen = false;
    }

    public string GetLabel()
    {
        return _label;
    }

    public string GetDescription()
    {
        return _description;
    }

    public CardType GetCardType()
    {
        return _cardType;
    }

    public bool GetHasBeenSeen()
    {
        return _hasBeenSeen;
    }

    public void SetHasBeenSeen(bool hasBeenSeen)
    {
        _hasBeenSeen = hasBeenSeen;
    }

    public string GetModelName()
    {
        return _modelName;
    }

    public void SetModelName(string modelName)
    {
        _modelName = modelName;
    }

    public void SetBackgroundColor(Color backGroundColor)
    {
        _backgroundColor = backGroundColor;
    }

    public Color GetBackgroundColor()
    {
        return _backgroundColor;
    }
}