
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : CardAnimation
{
    public ToInventoryAnimation(

        CardManager cardManager, CardMover cardMover, CardInventory cardInventory,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, cardMover, cardInventory, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }

    public override void MoveCardsStatic(CardNode pressedNode, CardNode rootNode)
    {
        // move in deck -> move out inventory

        _cardManager.AddToTopLevelMainPile(pressedNode);
        _cardMover.MoveCard(pressedNode, _playSpaceBottomLeft);

        if (pressedNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(pressedNode.Parent);
            _cardMover.MoveCard(pressedNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (pressedNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        // inventory open
        if (_cardInventory.InventoryIsOpenFlag)
        {
            

            // Set all cardnodes toplevel
            inventoryNode.IsTopLevel = true;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = true;
            }
            inventoryNode[1].IsTopLevel = true;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = true;
            }

            float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
            float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;

            float dialogueOffset = _playSpaceBottomLeft.x + 2 * _cardMover.GetBorder() + fannedCardSpace;
            FanCardsFromInventorySubcard(inventoryNode[0], dialogueOffset, fannedCardSpace);

            float keyOffset = _playSpaceBottomLeft.x + _cardMover.GetBorder();
            FanCardsFromInventorySubcard(inventoryNode[1], keyOffset, fannedCardSpace);
        }

        // inventory closed THIS NEEDS TO BE OUTSOURCED TO SOMEWHERE ELSE
        else
        {
            // Set no cardnodes toplevel
            inventoryNode[0].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = false;
            }
            inventoryNode[1].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = false;
            }
        }

        _cardMover.MoveCard(inventoryNode, new Vector2(_playSpaceTopRight.x, _playSpaceBottomLeft.y));

    }
    public void FanCardsFromInventorySubcard(CardNode inventorySubcard, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = inventorySubcard.Children.Count;

        _cardMover.MoveCard(inventorySubcard, new Vector2(startFanX + fannedCardSpace, _playSpaceBottomLeft.y));

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(inventorySubcard[i], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _playSpaceBottomLeft.y));
        }
    }

    public override async Task AnimateCards(CardNode mainToBe, CardNode backToBe, CardNode rootNode)
    {
        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}
