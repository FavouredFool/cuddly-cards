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
        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren - 1);

        int directionSign = fromRight ? 1 : -1;

        _cardMover.MoveCard(node, new Vector2(startOffset + directionSign * (totalChildren - index - 1) * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y));

        node.Body.SetHeightFloat(2 + (index * -0.01f));
        node.Body.transform.localRotation = Quaternion.Euler(0, 0, -directionSign *_cardMover.InventoryCardRotationAmount);
    }
}
