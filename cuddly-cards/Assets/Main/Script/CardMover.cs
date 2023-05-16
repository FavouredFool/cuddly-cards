using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    readonly float CARDHEIGHT = 0.005f;

    float _jitterAmount = 0.01f;

    int _cardCount = 0;

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

    //public void PileFromParenting(CardNode cardNode)
    //{
    //    if (cardNode.Parent != null)
    //    {
    //        Debug.LogError("Tried to create pile from non-root cardBody");
    //    }

    //    int cardCount = 0;

    //    cardNode.TraverseHeight(
    //        delegate (CardNode cardNode, int height)
    //        {
    //            SetHeight(cardNode.Body, height);
    //            cardCount += 1;
    //        }
    //        , 0
    //    );

    //    SetHeight(cardNode.Body, cardCount);
    //}

    public void PileFromParentingTransform(CardBody rootBody)
    {
        if (rootBody.transform.parent != _cardFolder)
        {
            Debug.LogError("Tried to create pile from non-root cardBody or root is null instead of cardFolder");
        }

        _cardCount = 0;

        SetHeightRecursive(rootBody.transform, 0);

        SetHeight(rootBody.transform, _cardCount);
    }

    int SetHeightRecursive(Transform bodyTransform, int height)
    {
        SetHeight(bodyTransform, height);
        
        _cardCount += 1;
        height = 0;

        foreach (Transform child in bodyTransform)
        {
            if (child.name == "CardContents")
            {
                continue;
            }

            height -= 1;
            height += SetHeightRecursive(child, height);
        }

        return height;
    }

    public void SetHeight(Transform bodyTransform, int height)
    {
        bodyTransform.localPosition = new Vector3(0, CARDHEIGHT * height, 0);
    }

}