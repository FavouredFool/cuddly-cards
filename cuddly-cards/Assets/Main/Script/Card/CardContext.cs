using static CardManager;
using static CardInfo;

public class CardContext
{
    string _label;
    string _description;
    CardType _cardType;

    bool _hasBeenSeen;


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
}