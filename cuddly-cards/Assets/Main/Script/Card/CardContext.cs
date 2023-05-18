using static CardManager;
using static CardInfo;

public class CardContext
{
    string _label;
    string _description;
    CardType _cardType;


    public CardContext(string label, string description, CardType cardType)
    {
        _label = label;
        _description = description;
        _cardType = cardType;
    }

    public string GetLabel()
    {
        return _label;
    }

    public CardType GetCardType()
    {
        return _cardType;
    }

}