using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAnimation : CardAnimation
{
    public OpenAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> _tweenXFuncFunc, Func<CardNode, int, Tween> _tweenYFuncFunc, Func<CardNode, float, Tween> _tweenZFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, _tweenXFuncFunc, _tweenYFuncFunc, _tweenZFuncFunc) { }


    public async override Task AnimateCards(CardNode activeNode, CardNode previousActiveNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(activeNode);
        DOTween.Sequence()
            .AppendInterval(_horizontalTime)
            .Append(_tweenYFunc(activeNode, 1));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        for(int i = 0; i < activeNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Children[i]);

            DOTween.Sequence()
                .Append(_tweenXFunc(activeNode.Children[i], i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(activeNode.Children[i], activeNode.Children[i].GetNodeCount(CardInfo.CardTraversal.CONTEXT)));
        }

        await DOTween.Sequence()
            .AppendInterval(_verticalTime + _horizontalTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        throw new NotImplementedException();
    }

    public override void MoveCardsStatic(CardNode activeNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(activeNode);
        _cardMover.MoveCard(activeNode, _playSpaceBottomLeft);

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);
            _cardMover.MoveCard(activeNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        for (int i = 0; i < activeNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Children[i]);
            _cardMover.MoveCard(activeNode.Children[i], new Vector2(i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset(), _playSpaceBottomLeft.y));
        }
    }

    public override void MoveCardsStaticNew(CardNode activeNode)
    {
        throw new NotImplementedException();
    }
}
