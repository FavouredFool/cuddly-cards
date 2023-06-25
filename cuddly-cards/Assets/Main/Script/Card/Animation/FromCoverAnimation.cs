
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class FromCoverAnimation : CardAnimation
{
    public FromCoverAnimation(

        CardManager cardManager, CardMover cardMover, CardInventory cardInventory,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFunc, Func<CardNode, int, Tween> __tweenYFunc, Func<CardNode, float, Tween> __tweenZFunc

        ) : base(cardManager, cardMover, cardInventory, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFunc, __tweenYFunc, __tweenZFunc) { }

    public override void MoveCardsStatic(CardNode pressedNode, CardNode rootNode)
    {
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

        for (int i = 0; i < pressedNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(pressedNode.Children[i]);
            _cardMover.MoveCard(pressedNode.Children[i], new Vector2(i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset(), _playSpaceBottomLeft.y));
        }
    }

    public override async Task AnimateCards(CardNode mainNode, CardNode previousActiveNode, CardNode rootNode)
    {
        _cardManager.AddToTopLevelMainPile(rootNode);
        DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_tweenXFunc(rootNode, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_tweenYFunc(rootNode, 1));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];
            _cardManager.AddToTopLevelMainPile(childNode);

            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_tweenXFunc(childNode, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(childNode, i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(childNode, childNode.GetNodeCount(CardTraversal.CONTEXT)));
        }

        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}