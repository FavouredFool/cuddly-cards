using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class ToCoverAnimation : CardAnimation
{
    public ToCoverAnimation(CardManager cardManager, CardMover cardMover) : base(cardManager, cardMover){}

    public async override Task AnimateCards(CardNode activeNode, CardNode previousMainNode, CardNode rootNode)
    {
        // -------------- CHILDREN ---------------------

        List<CardNode> children = previousMainNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            _cardManager.AddToTopLevelMainPile(child);

            DOTween.Sequence()
                .Append(_cardMover.TweenY(child, child.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .Append(_cardMover.TweenX(child, _cardMover.GetPlaySpaceBottomLeft().x))
                .AppendInterval(2 * _cardMover.GetWaitTime() + _cardMover.GetHorizontalTime())
                .Append(_cardMover.TweenX(child, _cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f));
        }

        // -------------- MAIN ---------------------

        _cardManager.AddToTopLevelMainPile(previousMainNode);
        DOTween.Sequence()
            .Append(_cardMover.TweenY(previousMainNode, previousMainNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
            .AppendInterval(2 * _cardMover.GetHorizontalTime() + 2 * _cardMover.GetWaitTime())
            .Append(_cardMover.TweenX(previousMainNode, _cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f));

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
                .Append(_cardMover.TweenY(node, node.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .AppendInterval(_cardMover.GetHorizontalTime() + _cardMover.GetWaitTime())
                .Append(_cardMover.TweenZ(node, _cardMover.GetPlaySpaceBottomLeft().y))
                .AppendInterval(_cardMover.GetWaitTime())
                .Append(_cardMover.TweenX(node, _cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f));
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
                .Append(_cardMover.TweenY(node, node.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT)))
                .Append(_cardMover.TweenX(node, _cardMover.GetPlaySpaceBottomLeft().x))
                .AppendInterval(_cardMover.GetWaitTime())
                .Append(_cardMover.TweenZ(node, _cardMover.GetPlaySpaceBottomLeft().y))
                .AppendInterval(_cardMover.GetWaitTime())
                .Append(_cardMover.TweenX(node, _cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f));
        }

        // THE EMPTY ONCOMPLETE NEEDS TO BE THERE, OTHERWISE IT WILL NOT WORK!
        await DOTween.Sequence()
            .AppendInterval(_cardMover.GetVerticalTime() * 2 + _cardMover.GetHorizontalTime() * 3 + 2 * _cardMover.GetWaitTime() + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }
}
