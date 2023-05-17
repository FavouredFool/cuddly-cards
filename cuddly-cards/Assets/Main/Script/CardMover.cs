using UnityEngine;
using System.Collections.Generic;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    float _jitterAmount = 0.01f;

    int _cardCount = 0;

    public void ParentCards(ContextNode rootNode, List<BodyNode> topLevelNodes)
    {
        rootNode.Traverse(
            delegate (ContextNode contextNode)
            {
                // This sets both the Unity-Hierachie AND the implied bodyhierachie in code through ChildrenBody and ParentBody

                // reset children: ONLY WORKS IF THE TRAVERSE-ORDER STAYS THE SAME
                // IF THE CHILDREN EVER GET TRAVERSED BEFORE THE PARENT; THIS DIES

                BodyNode bodyNode = contextNode.CBody;

                bodyNode.Clear();

                // parent
                BodyNode parent = contextNode.Parent?.CBody;

                // If the node is top level, cut off any parenting
                if (topLevelNodes.Contains(bodyNode)) parent = null;

                // Set parent in both unity- and body-hierachie
                bodyNode.Parent = parent;
                bodyNode.Body.transform.parent = parent?.Body.transform;
                bodyNode.Body.transform.parent ??= _cardFolder;

                // Set child of parent
                parent?.Children.Add(bodyNode);

                return true;
            }
        );
    }

    public void PileFromParenting(BodyNode topLevelBodyNode)
    {
        if (topLevelBodyNode.Parent != null)
        {
            Debug.LogError("Tried to create pile from non-topLevel cardBody");
        }

        _cardCount = 0;

        SetHeightRecursive(topLevelBodyNode, 0);

        topLevelBodyNode.Body.SetHeight(_cardCount);
    }

    int SetHeightRecursive(BodyNode bodyNode, int height)
    {
        // this could also go inside of CardBody, but then i'd need a different solution for the _cardCount
        bodyNode.Body.SetHeight(height);
        
        _cardCount += 1;
        height = 0;

        foreach (BodyNode child in bodyNode.Children)
        {
            height -= 1;
            height += SetHeightRecursive(child, height);
        }

        return height;
    }

    public void MoveCardRandom(BodyNode card)
    {
        MoveCard(card, Random.insideUnitCircle * 2);
    }

    public void MoveCard(BodyNode card, Vector2 position)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
    }
}