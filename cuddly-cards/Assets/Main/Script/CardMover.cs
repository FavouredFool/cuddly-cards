using UnityEngine;
using System.Collections.Generic;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    float _jitterAmount = 0.01f;

    int _cardCount = 0;

    public void ParentCards(CardNode rootNode, List<CardNode> topLevelNodes)
    {
        rootNode.TraverseContext(
            delegate (CardNode cardNode)
            {
                CardNode parent = cardNode.Parent;

                // If the node is top level, cut off any parenting
                if (topLevelNodes.Contains(cardNode)) parent = null;

                cardNode.Body.transform.parent = parent?.Body.transform;
                cardNode.Body.transform.parent ??= _cardFolder;

                return true;
            }
        );
    }

    public void PileFromParenting(CardNode topLevelNode)
    {
        if (!topLevelNode.IsTopLevel)
        {
            Debug.LogError("Tried to create pile from non-topLevel cardBody");
        }

        _cardCount = 0;

        SetHeightRecursive(topLevelNode, 0);

        topLevelNode.Body.SetHeight(_cardCount);
    }

    int SetHeightRecursive(CardNode node, int height)
    {
        // this could also go inside of CardBody, but then i'd need a different solution for the _cardCount
        node.Body.SetHeight(height);
        
        _cardCount += 1;
        height = 0;

        foreach (CardNode child in node.Children)
        {
            if (child.IsTopLevel)
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
}