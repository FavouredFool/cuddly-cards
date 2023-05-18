using UnityEngine;
using System.Collections.Generic;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    readonly float JITTERAMOUNT = 0.01f;

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
            node.Body.transform.localPosition = new Vector3(jitter.x * JITTERAMOUNT, node.Body.transform.localPosition.y, jitter.y * JITTERAMOUNT);
            return true;
        });
    }



    public void MoveCardRandom(CardNode card)
    {
        MoveCard(card, Random.insideUnitCircle * 2, 0);
    }

    public void MoveCard(CardNode card, Vector2 position, float rotation)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
        card.Body.transform.localRotation = Quaternion.Euler(new Vector3(0, rotation, 0));
    }

    public void MoveCardsForLayout(CardNode mainNode, CardNode rootNode)
    {
        MoveCard(mainNode, new Vector2(-2.5f, 0), 0);

        if (mainNode != rootNode)
        {
            MoveCard(mainNode.Parent, new Vector2(-2.5f, 2.5f), 90);

            if (mainNode.Parent != rootNode)
            {
                MoveCard(rootNode, new Vector2(2.875f, 2.5f), 90);
            }
        }

        for (int i = 0; i < mainNode.Children.Count; i++)
        {
            MoveCard(mainNode.Children[i], new Vector2(i * 1.125f - 1f, 0), 0);
        }
    }

    public void ResetPositionAndRotation(CardNode rootNode)
    {
        rootNode.TraverseContext(delegate (CardNode node)
        {
            node.Body.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return true;
        });
    }
}