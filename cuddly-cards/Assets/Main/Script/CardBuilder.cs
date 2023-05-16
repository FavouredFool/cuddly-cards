using UnityEngine;

public class CardBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    Transform _cardFolder;


    public void BuildAllCards(CardNode rootNode)
    {
        rootNode.Traverse(
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
        CardBody cardBody = Instantiate(_cardBlueprint, Vector3.zero, Quaternion.identity, _cardFolder).GetComponent<CardBody>();
        cardBody.SetLabel(cardContext.GetLabel());
        cardBody.gameObject.name = "Card: \"" + cardContext.GetLabel() + "\"";
        return cardBody;
    }
}