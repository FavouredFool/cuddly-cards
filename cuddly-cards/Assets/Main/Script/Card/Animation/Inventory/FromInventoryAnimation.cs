
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    public FromInventoryAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }


    public override void MoveCardsStatic(CardNode activeNode)
    {
        CardNode inventoryNode = _cardInventory.GetInventoryNode();

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
        
        _cardMover.MoveCard(inventoryNode, new Vector2(_playSpaceTopRight.x, _playSpaceBottomLeft.y));
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        entireSequence.Join(DOTween.Sequence()
            .Append(inventoryNode.Body.transform.DOMoveY(inventoryNode.GetNodeCount(CardTraversal.CONTEXT) * CardInfo.CARDHEIGHT, _horizontalTime)));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            float generalStartOffset = _playSpaceBottomLeft.x + (1 + (1 - i)) * _cardMover.GetBorder() + (1 - i) * fannedCardSpace;

            CardNode subNode = inventoryNode[i];

            subNode.IsTopLevel = true;

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenXFunc(subNode, _playSpaceTopRight.x))
                .Join(subNode.Body.transform.DOMoveY(subNode.GetNodeCountUpToNodeInPile(inventoryNode, CardTraversal.CONTEXT) * CardInfo.CARDHEIGHT, _horizontalTime).SetEase(_cardMover.GetHorizontalEase()))
                .Join(subNode.Body.transform.DOLocalRotate(new Vector3(0, 0, 0), _waitTime).SetEase(_cardMover.GetHorizontalEase())));

            int totalChildren = subNode.Children.Count;

            for (int j = 0; j < totalChildren; j++)
            {
                CardNode childNode = subNode[j];

                entireSequence.Join(DOTween.Sequence()
                .Append(_tweenXFunc(childNode, _playSpaceTopRight.x))
                .Join(childNode.Body.transform.DOMoveY(childNode.GetNodeCountUpToNodeInPile(inventoryNode, CardTraversal.CONTEXT) * CardInfo.CARDHEIGHT, _horizontalTime).SetEase(_cardMover.GetHorizontalEase()))
                .Join(childNode.Body.transform.DOLocalRotate(new Vector3(0, 0, 0), _waitTime).SetEase(_cardMover.GetHorizontalEase())));
            }
        }

        return entireSequence;
    }
}
