using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    [SerializeField]
    string _excludeName;

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

    public void PileFromParenting(CardBody rootBody)
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
            if (child.name == _excludeName)
            {
                continue;
            }

            height -= 1;
            height += SetHeightRecursive(child, height);
        }

        return height;
    }

    public void MoveCardRandom(CardNode card)
    {
        MoveCard(card, Random.insideUnitCircle * 2);
    }

    public void MoveCard(CardNode card, Vector2 position)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
    }

    public void SetHeight(Transform bodyTransform, int height)
    {
        bodyTransform.localPosition = new Vector3(
            bodyTransform.localPosition.x,
            CardInfo.CARDHEIGHT * height,
            bodyTransform.localPosition.z
        );
    }

}