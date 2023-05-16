using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    readonly float CARDHEIGHT = 0.005f;

    float _jitterAmount = 0.01f;

    public void ParentCards(TreeNode<CardContext> treeRoot)
    {
        treeRoot.Traverse(
            delegate (TreeNode<CardContext> treeNode)
            {
                CardContext context = treeNode.Data;
                CardBody body = context.GetCardBody();

                Transform parent = treeNode.Parent?.Data.GetCardBody().transform;
                body.transform.parent = parent == null ? _cardFolder : parent;
                return true;
            }
        );
    }
}