
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildAnimation : CardAnimation
{
    public ChildAnimation(

        CardManager cardManager, CardMover cardMover,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, cardMover, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }


    public override async Task AnimateCards(CardNode mainToBe, CardNode backToBe, CardNode rootNode)
    {
        CardNode discardToBe = backToBe.Parent;
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        List<CardNode> childsToBe = mainToBe.Children;
        List<CardNode> previousChilds = backToBe.Children;

        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevelMainPile(newChild);

            DOTween.Sequence()
                .Append(_tweenYFunc(newChild, newChild.GetNodeCountUpToNodeInPile(backToBe, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(newChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(newChild, newChild.Parent.Children.IndexOf(newChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(newChild, newChild.GetNodeCount(CardTraversal.CONTEXT)));
        }


        // ------------- MAIN TO BE ----------------

        _cardManager.AddToTopLevelMainPile(mainToBe);
        DOTween.Sequence()
            .Append(_tweenYFunc(mainToBe, mainToBe.GetNodeCountUpToNodeInPile(backToBe, CardTraversal.CONTEXT)))
            .Append(_tweenXFunc(mainToBe, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_tweenYFunc(mainToBe, 1));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];

            if (previousChild == mainToBe)
            {
                continue;
            }

            _cardManager.AddToTopLevelMainPile(previousChild);

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            DOTween.Sequence()
                .Append(_tweenYFunc(previousChild, previousChild.GetNodeCountUpToNodeInPile(backToBe, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(previousChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenZFunc(previousChild, _playSpaceTopRight.y))
                .Append(_tweenYFunc(previousChild, height));

        }

        // ------------- BackToBe ----------------

        _cardManager.AddToTopLevelMainPile(backToBe);
        DOTween.Sequence()
            .Append(_tweenYFunc(backToBe, backToBe.GetNodeCount(CardTraversal.CONTEXT)))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_tweenZFunc(backToBe, _playSpaceTopRight.y))
            .Append(_tweenYFunc(backToBe, backToBe.GetNodeCount(CardTraversal.CONTEXT) - mainToBe.GetNodeCount(CardTraversal.CONTEXT)));


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

            DOTween.Sequence()
                .Append(_tweenYFunc(discardToBe, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_tweenXFunc(discardToBe, _playSpaceTopRight.x));
        }
        else if (discardToBe != null)
        {
            _cardManager.AddToTopLevelMainPile(discardToBe);
            DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(_tweenXFunc(discardToBe, _playSpaceTopRight.x));
        }

        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}
