
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    public FromInventoryAnimation(CardManager cardManager) : base(cardManager) { }

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
