
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildAnimation : CardAnimation
{
    public ChildAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }

    public override void MoveCardsStatic(CardNode pressedNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
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

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.GetRootNode();

        CardNode discardToBe = previousActiveNode.Parent;
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = previousActiveNode.Children;

        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevelMainPile(newChild);

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenYFunc(newChild, newChild.GetNodeCountUpToNodeInPile(previousActiveNode, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(newChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(newChild, newChild.Parent.Children.IndexOf(newChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(newChild, newChild.GetNodeCount(CardTraversal.CONTEXT))));
        }


        // ------------- MAIN TO BE ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_tweenYFunc(activeNode, activeNode.GetNodeCountUpToNodeInPile(previousActiveNode, CardTraversal.CONTEXT)))
            .Append(_tweenXFunc(activeNode, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_tweenYFunc(activeNode, 1)));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];

            if (previousChild == activeNode)
            {
                continue;
            }

            _cardManager.AddToTopLevelMainPile(previousChild);

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenYFunc(previousChild, previousChild.GetNodeCountUpToNodeInPile(previousActiveNode, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(previousChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenZFunc(previousChild, _playSpaceTopRight.y))
                .Append(_tweenYFunc(previousChild, height)));

        }

        // ------------- BackToBe ----------------

        _cardManager.AddToTopLevelMainPile(previousActiveNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_tweenYFunc(previousActiveNode, previousActiveNode.GetNodeCount(CardTraversal.CONTEXT)))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_tweenZFunc(previousActiveNode, _playSpaceTopRight.y))
            .Append(_tweenYFunc(previousActiveNode, previousActiveNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT))));


        // ------------- Discard & DiscardToBe ----------------

        if (discard != null)
        {
            _cardManager.AddToTopLevelMainPile(discard);
            _cardManager.AddToTopLevelMainPile(discardToBe);

            // height needs to be calculated before the deck is split in two, because otherwise new top-levels would be overlooked (this is a bit ugly)
            int discardHeight = discard.GetNodeCount(CardTraversal.BODY) + discardToBe.GetNodeCount(CardTraversal.BODY);
            int discardToBeHeight = discardToBe.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.BODY);

            List<CardNode> lowerTopMostCardsRoot = discardToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevelMainPile(node);
            }

            _tweenYFunc(rootNode, discardHeight);

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenYFunc(discardToBe, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_tweenXFunc(discardToBe, _playSpaceTopRight.x)));
        }
        else if (discardToBe != null)
        {
            _cardManager.AddToTopLevelMainPile(discardToBe);
            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(_tweenXFunc(discardToBe, _playSpaceTopRight.x)));
        }

        return entireSequence;
    }
}
