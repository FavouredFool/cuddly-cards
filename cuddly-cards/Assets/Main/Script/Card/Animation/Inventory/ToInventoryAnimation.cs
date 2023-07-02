
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : InventoryAnimation
{
    public ToInventoryAnimation(CardManager cardManager) : base(cardManager) { }

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

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        entireSequence.Join(DOTween.Sequence()
            .Append(_tweenYFunc(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT) + 1))
            .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
            .Append(inventoryNode.Body.transform.DOMoveY(CardInfo.CARDHEIGHT, _horizontalTime)));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            float generalStartOffset = _playSpaceBottomLeft.x + (1 + (1 - i)) * _cardMover.GetBorder() + (1 - i) * fannedCardSpace;

            CardNode subNode = inventoryNode[i];

            subNode.IsTopLevel = true;

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenYFunc(subNode, subNode.GetNodeCountUpToNodeInPile(inventoryNode, CardTraversal.CONTEXT) + 1))
                .Append(_tweenXFunc(subNode, generalStartOffset))
                .Append(subNode.Body.transform.DOLocalRotate(new Vector3(0, 0, -_cardMover.GetInventoryCardRotationAmount()), _waitTime))
                .Append(subNode.Body.transform.DOMove(new Vector3(generalStartOffset + fannedCardSpace, 2 * CardInfo.CARDHEIGHT, subNode.Body.transform.position.z), _horizontalTime)));

            int totalChildren = subNode.Children.Count;

            for (int j = 0; j < totalChildren; j++)
            {
                CardNode childNode = subNode[j];

                float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren);

                entireSequence.Join(DOTween.Sequence()
                    .Append(_tweenYFunc(childNode, childNode.GetNodeCountUpToNodeInPile(inventoryNode, CardTraversal.CONTEXT) + 1))
                    .Append(_tweenXFunc(childNode, generalStartOffset))
                    .Append(childNode.Body.transform.DOLocalRotate(new Vector3(0, 0, -_cardMover.GetInventoryCardRotationAmount()), _waitTime))
                    .Append(childNode.Body.transform.DOMove(new Vector3(generalStartOffset + (totalChildren - 1 - j) * CardInfo.CARDWIDTH * cardPercentage, 2 * CardInfo.CARDHEIGHT, childNode.Body.transform.position.z), _horizontalTime)));
            }
        }

        return entireSequence;
    }
}
