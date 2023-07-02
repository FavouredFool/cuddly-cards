using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public class CardBuilder : MonoBehaviour
{
    [Header("Card Blueprint")]
    [SerializeField]
    GameObject _cardBlueprint;

    [Header("All Cardtypes")]
    [SerializeField]
    List<CardScriptableType> _types;

    public void BuildAllCards(CardNode rootNode, Transform folder)
    {
        rootNode.TraverseChildren(CardTraversal.CONTEXT,
            delegate (CardNode cardNode)
            {
                CardContext context = cardNode.Context;
                cardNode.Body = BuildCardBody(context, folder);
                return true;
            }
        );
    }

    public CardBody BuildCardBody(CardContext cardContext, Transform folder)
    {
        CardBody body = GameObject.Instantiate(_cardBlueprint, Vector3.zero, Quaternion.identity, folder).GetComponent<CardBody>();

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