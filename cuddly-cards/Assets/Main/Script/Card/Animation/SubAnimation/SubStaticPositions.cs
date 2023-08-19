using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class SubStaticPositions
{
    readonly CardMover _cardMover;
    readonly CardManager _cardManager;
    readonly CardInventory _cardInventory;

    readonly Vector2 _playSpaceBottomLeft;
    readonly Vector2 _playSpaceTopRight;

    public SubStaticPositions(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = _cardManager.CardMover;
        _cardInventory = _cardManager.CardInventory;
        _playSpaceBottomLeft = _cardMover.GetPlaySpaceBottomLeft();
        _playSpaceTopRight = _cardMover.GetPlaySpaceTopRight();
    }

    public void FanOutCard(CardNode node, int index, int totalChildren, bool fromRight)
    {
        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = (totalSpace - 2 * _cardMover.Border);
        float startOffset = fromRight ? _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border : _cardMover.PlaySpaceTopRight.x - _cardMover.Border;

        Vector2 cardPosition;

        int directionSign = fromRight ? 1 : -1;


        if (totalChildren > 1)
        {
            float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren - 1);
            cardPosition = new Vector2(startOffset + directionSign * (totalChildren - index - 1) * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y);
        }
        else
        {
            cardPosition = new Vector2(startOffset + directionSign * fannedCardSpace, _cardMover.GetPlaySpaceBottomLeft().y);
        }
         
        _cardMover.MoveCard(node, cardPosition);

        node.Body.SetHeightFloat(2 - (index * 0.01f));
        node.Body.transform.localRotation = Quaternion.Euler(0, 0, -directionSign *_cardMover.InventoryCardRotationAmount);
    }

    public void SetNode(CardNode node, Vector2 position)
    {
        _cardManager.AddToTopLevel(node);
        node.Body.SetHeight(node.GetNodeCount(CardTraversal.BODY));
        _cardMover.MoveCard(node, position);
    }

    public void SetChild(CardNode child, int index)
    {
        _cardManager.AddToTopLevel(child);
        child.Body.SetHeight(child.GetNodeCount(CardTraversal.BODY));
        _cardMover.MoveCard(child, new Vector2(index * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetBaseBackRoot(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;

        SetNode(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            SetNode(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                SetNode(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }
}
