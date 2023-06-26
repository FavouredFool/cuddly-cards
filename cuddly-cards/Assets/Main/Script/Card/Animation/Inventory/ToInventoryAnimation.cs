
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : CardAnimation
{
    public ToInventoryAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }


    public override void MoveCardsStatic(CardNode activeNode, CardNode rootNode)
    {
        CardNode inventoryNode = _cardInventory.GetInventoryNode();


        // Set all cardnodes toplevel
        inventoryNode[0].IsTopLevel = true;
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
        FanCardsFromInventorySubcardStatic(inventoryNode[0], dialogueOffset, fannedCardSpace);

        float keyOffset = _playSpaceBottomLeft.x + _cardMover.GetBorder();
        FanCardsFromInventorySubcardStatic(inventoryNode[1], keyOffset, fannedCardSpace);
        
        _cardMover.MoveCard(inventoryNode, new Vector2(_playSpaceTopRight.x, _playSpaceBottomLeft.y));
    }

    public void FanCardsFromInventorySubcardStatic(CardNode inventorySubcard, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = inventorySubcard.Children.Count;

        _cardMover.MoveCard(inventorySubcard, new Vector2(startFanX + fannedCardSpace, _playSpaceBottomLeft.y));

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(inventorySubcard[totalChildCards - 1 - i], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _playSpaceBottomLeft.y));
        }
    }

    public override async Task AnimateCards(CardNode mainToBe, CardNode backToBe, CardNode rootNode)
    {
        float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;


        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            float generalStartOffset = _playSpaceBottomLeft.x + (1 + (1- i)) * _cardMover.GetBorder() + (1 - i) * fannedCardSpace;

            CardNode subNode = inventoryNode[i];

            subNode.IsTopLevel = true;

            DOTween.Sequence()
                .Append(_tweenXFunc(subNode, generalStartOffset))
                .Append(subNode.Body.transform.DORotate(new Vector3(0, 0, -_cardMover.GetInventoryCardRotationAmount()), _waitTime))
                .Append(subNode.Body.transform.DOMove(new Vector3(generalStartOffset + fannedCardSpace, 2 * CardInfo.CARDHEIGHT, subNode.Body.transform.position.z), _horizontalTime + _verticalTime));

            int totalChildren = subNode.Children.Count;

            for (int j = 0; j < totalChildren; j++)
            {
                CardNode childNode = subNode[j];

                float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren);

                DOTween.Sequence()
                    .Append(_tweenXFunc(childNode, generalStartOffset))
                    .Append(childNode.Body.transform.DOLocalRotate(new Vector3(0, 0, -_cardMover.GetInventoryCardRotationAmount()), _waitTime))
                    .Append(childNode.Body.transform.DOMove(new Vector3(generalStartOffset + (totalChildren - 1 - j) * CardInfo.CARDWIDTH * cardPercentage, 2 * CardInfo.CARDHEIGHT, childNode.Body.transform.position.z), _horizontalTime + _verticalTime));
            }

        }


        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}
