using UnityEngine;
using System.Collections.Generic;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    float _jitterAmount = 0.01f;

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

        topLevelNode.SetHeightRecursive(0);
        topLevelNode.Body.SetHeight(topLevelNode.NodeCount());

        // This might be performance-intensive due to being heavily nested
        topLevelNode.TraverseBody(delegate (CardNode node)
        {
            Vector2 jitter = Random.insideUnitCircle;
            node.Body.transform.localPosition = new Vector3(jitter.x * _jitterAmount, node.Body.transform.localPosition.y, jitter.y * _jitterAmount);
            return true;
        });
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