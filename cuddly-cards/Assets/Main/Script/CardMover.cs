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
        rootNode.Traverse(
            delegate (CardNode cardNode)
            {
                // This sets both the Unity-Hierachie AND the implied bodyhierachie in code through ChildrenBody and ParentBody

                // reset children: ONLY WORKS IF THE TRAVERSE-ORDER STAYS THE SAME
                // IF THE CHILDREN EVER GET TRAVERSED BEFORE THE PARENT; THIS DIES
                
                cardNode.Body.ChildrenBody.Clear();

                // parent
                CardBody parent = cardNode.ParentNode?.Body;

                // If the node is top level, cut off any parenting
                if (topLevelNodes.Contains(cardNode)) parent = null;

                // Set parent in both unity- and body-hierachie
                cardNode.Body.ParentBody = parent;
                cardNode.Body.transform.parent = parent?.transform;
                cardNode.Body.transform.parent ??= _cardFolder;

                // Set child of parent
                parent?.ChildrenBody.Add(cardNode.Body);

                return true;
            }
        );
    }

    public void PileFromParenting(CardBody topLevelBody)
    {
        if (topLevelBody.transform.parent != _cardFolder)
        {
            Debug.LogError("Tried to create pile from non-topLevel cardBody or root is null instead of cardFolder");
        }

        _cardCount = 0;

        SetHeightRecursive(topLevelBody, 0);

        topLevelBody.SetHeight(_cardCount);
    }

    int SetHeightRecursive(CardBody cardBody, int height)
    {
        // this could also go inside of CardBody, but then i'd need a different solution for the _cardCount
        cardBody.SetHeight(height);
        
        _cardCount += 1;
        height = 0;

        foreach (CardBody child in cardBody.ChildrenBody)
        {
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