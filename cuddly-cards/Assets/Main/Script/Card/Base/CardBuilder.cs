using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public class CardBuilder : MonoBehaviour
{
    [Header("Card Blueprint")]
    [SerializeField] GameObject _cardBlueprint;

    [Header("All Cardtypes")]
    [SerializeField] List<CardScriptableType> _types;

    public void BuildAllCards(CardNode rootNode, Transform folder)
    {
        rootNode.TraverseChildren(CardTraversal.CONTEXT,
            delegate (CardNode cardNode)
            {
                CardContext context = cardNode.Context;
                cardNode.Body = BuildCardBody(context, cardNode, folder);
                return true;
            }
        );
    }

    public CardBody BuildCardBody(CardContext cardContext, CardNode nodeReference, Transform folder)
    {
        CardBody body = GameObject.Instantiate(_cardBlueprint, Vector3.zero, Quaternion.identity, folder).GetComponent<CardBody>();

        body.SetLabel(cardContext.Label);
        body.gameObject.name = "Card: \"" + cardContext.Label + "\"";
        body.CardReferenceNode = nodeReference;

        CardScriptableType type = _types.FirstOrDefault(e => e.GetCardType().Equals(cardContext.CardType));

        if (type == null)
        {
            Debug.LogError("Found no fitting type for card: " + cardContext.CardType);
        }

        body.SetColor(type.GetCardColor());
        body.SetIcon(type.GetCardIcon());

        return body;
    }
}