using static CardManager;

public class CardContext
{
    string _label;
    CardType _cardType;
    CardBody _cardBody;
    


    public CardContext(string label, CardType cardType)
    {
        _label = label;
        _cardType = cardType;
    }

    public string GetLabel()
    {
        return _label;
    }

    public void SetCardBody(CardBody cardBody)
    {
        _cardBody = cardBody;
    }

    public CardBody GetCardBody()
    {
        return _cardBody;
    }


}