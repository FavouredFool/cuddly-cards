using UnityEngine;

public class CardBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject _cardBlueprint;

    [SerializeField]
    Transform _cardFolder;


    public void BuildAllCards(TreeNode<CardContext> treeRoot)
    {
        treeRoot.Traverse(
            delegate (TreeNode<CardContext> treeNode)
            {
                CardContext context = treeNode.Data;
                context.SetCardBody(BuildCard(context));
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