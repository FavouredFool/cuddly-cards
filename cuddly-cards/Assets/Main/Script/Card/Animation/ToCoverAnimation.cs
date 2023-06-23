using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using static CardInfo;

public class ToCoverAnimation : CardAnimation
{
    public ToCoverAnimation(

        CardManager cardManager, CardMover cardMover,
        float waitTime, float horizontalWaitTime,float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> _tweenXFuncFunc, Func<CardNode, int, Tween> _tweenYFuncFunc, Func<CardNode, float, Tween> _tweenZFuncFunc
        
        ) : base(cardManager, cardMover, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, _tweenXFuncFunc, _tweenYFuncFunc, _tweenZFuncFunc){}

    public async override Task AnimateCards(CardNode activeNode, CardNode previousMainNode, CardNode rootNode)
    {
        // -------------- CHILDREN ---------------------

        List<CardNode> children = previousMainNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            _cardManager.AddToTopLevelMainPile(child);

            DOTween.Sequence()
                .Append(_tweenYFunc(child, child.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(child, _playSpaceBottomLeft.x))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(_tweenXFunc(child, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }

        // -------------- MAIN ---------------------

        _cardManager.AddToTopLevelMainPile(previousMainNode);
        DOTween.Sequence()
            .Append(_tweenYFunc(previousMainNode, previousMainNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(_tweenXFunc(previousMainNode, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));

        // -------------- BACK ---------------------

        CardNode backNode = previousMainNode.Parent;
        _cardManager.AddToTopLevelMainPile(backNode);

        List<CardNode> lowerTopMostCardsBack = previousMainNode.GetTopNodesBelowNodeInPile(backNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        foreach (CardNode node in animatingNodesBack)
        {
            DOTween.Sequence()
                .Append(_tweenYFunc(node, node.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_tweenZFunc(node, _playSpaceBottomLeft.y))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }


        // -------------- ROOT ---------------------

        _cardManager.AddToTopLevelMainPile(rootNode);
        List<CardNode> lowerTopMostCardsRoot = backNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsRoot)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
        animatingNodesRoot.Add(rootNode);

        foreach (CardNode node in animatingNodesRoot)
        {
            DOTween.Sequence()
                .Append(_tweenYFunc(node, node.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(node, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenZFunc(node, _playSpaceBottomLeft.y))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }

        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 3 + 2 * _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}
