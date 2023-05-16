using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    readonly float CARDHEIGHT = 0.005f;

    float _jitterAmount = 0.01f;

    public void ParentCards(CardNode nodeRoot)
    {
        nodeRoot.Traverse(
            delegate (CardNode cardNode)
            {
                Transform parent = cardNode.Parent?.Body.transform;
                cardNode.Body.transform.parent = parent == null ? _cardFolder : parent;
                return true;
            }
        );
    }
}