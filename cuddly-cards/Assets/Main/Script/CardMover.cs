using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    readonly float CARDHEIGHT = 0.005f;

    float _jitterAmount = 0.01f;

    public void ParentCards(CardNode rootNode)
    {
        rootNode.Traverse(
            delegate (CardNode cardNode)
            {
                Transform parent = cardNode.Parent?.Body.transform;
                cardNode.Body.transform.parent = parent == null ? _cardFolder : parent;
                return true;
            }
        );
    }

    public void PileFromParenting(CardNode cardNode)
    {
        if (cardNode.Parent != null)
        {
            Debug.LogError("Tried to create pile from non-root cardBody");
        }

        int cardCount = 0;

        cardNode.TraverseHeight(
            delegate (CardNode cardNode, int height)
            {
                SetHeight(cardNode.Body, height);
                cardCount += 1;
            }
            , 0
        );

        SetHeight(cardNode.Body, cardCount);
    }

    public void SetHeight(CardBody body, int height)
    {
        body.SetPosition(new Vector3(0, CARDHEIGHT * height, 0));
    }
}