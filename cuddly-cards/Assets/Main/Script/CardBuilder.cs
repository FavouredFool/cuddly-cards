using UnityEngine;

public class CardBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    Transform _cardFolder;


    public void BuildAllCards(ContextNode rootNode)
    {
        rootNode.Traverse(
            delegate (ContextNode cardNode)
            {
                CardContext context = cardNode.Context;
                cardNode.CBody = BuildCard(context);
                return true;
            }
        );
    }

    public BodyNode BuildCard(CardContext cardContext)
    {
        BodyNode node = new(Instantiate(_cardBlueprint, Vector3.zero, Quaternion.identity, _cardFolder).GetComponent<CardBody>());
        node.Body.SetLabel(cardContext.GetLabel());
        node.Body.gameObject.name = "Card: \"" + cardContext.GetLabel() + "\"";
        return node;
    }
}