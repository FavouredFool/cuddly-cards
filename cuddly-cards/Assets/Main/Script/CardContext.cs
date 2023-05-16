using static CardManager;

public class CardContext
{
    string _label;
    CardType _cardType;   


    public CardContext(string label, CardType cardType)
    {
        _label = label;
        _cardType = cardType;
    }

    public string GetLabel()
    {
        return _label;
    }

}