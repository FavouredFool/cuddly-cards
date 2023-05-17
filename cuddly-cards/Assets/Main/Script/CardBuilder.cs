using UnityEngine;

public class CardBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    Transform _cardFolder;


    public void BuildAllCards(CardNode rootNode)
    {
        rootNode.TraverseContext(
            delegate (CardNode cardNode)
            {
                CardContext context = cardNode.Context;
                cardNode.Body = BuildCard(context);
                return true;
            }
        );
    }

    public CardBody BuildCard(CardContext cardContext)
    {
        CardBody body = Instantiate(_cardBlueprint, Vector3.zero, Quaternion.identity, _cardFolder).GetComponent<CardBody>();
        body.SetLabel(cardContext.GetLabel());
        body.gameObject.name = "Card: \"" + cardContext.GetLabel() + "\"";
        return body;
    }
}