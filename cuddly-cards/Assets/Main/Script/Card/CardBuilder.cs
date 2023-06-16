using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public class CardBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    List<CardScriptableType> _types;

    [SerializeField]
    Transform _cardFolder;

    public void BuildAllCards(CardNode rootNode)
    {
        rootNode.TraverseChildren(CardTraversal.CONTEXT,
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

        CardScriptableType type = _types.Where(e => e.GetCardType().Equals(cardContext.GetCardType())).FirstOrDefault();

        if (type == null)
        {
            Debug.LogError("Found no fitting type for card: " + cardContext.GetCardType());
        }

        body.SetColor(type.GetCardColor());
        body.SetIcon(type.GetCardIcon());

        return body;
    }
}