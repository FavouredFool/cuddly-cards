
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackAnimation : CardAnimation
{
    public BackAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backToBe = activeNode.Parent;
        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = previousActiveNode.Children;

        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevelMainPile(newChild);


            if (previousActiveNode == newChild)
            {
                // ------------- PREVIOUS MAIN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_tweenYFunc(previousActiveNode, previousActiveNode.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT)))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_tweenXFunc(previousActiveNode, newChild.Parent.Children.IndexOf(newChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                    .Append(_tweenYFunc(previousActiveNode, previousActiveNode.GetNodeCountUpToNodeInPile(previousActiveNode, CardTraversal.CONTEXT))));

                // ------------- PREVIOUS CHILDREN ----------------
                for (int j = previousChilds.Count - 1; j >= 0; j--)
                {
                    CardNode oldChild = previousChilds[j];

                    _cardManager.AddToTopLevelMainPile(oldChild);

                    entireSequence.Join(DOTween.Sequence()
                        .Append(_tweenYFunc(oldChild, oldChild.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT)))
                        .Append(_tweenXFunc(oldChild, _playSpaceBottomLeft.x))
                        .AppendInterval(_waitTime)
                        .Append(_tweenXFunc(oldChild, newChild.Parent.Children.IndexOf(newChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                        .Append(_tweenYFunc(oldChild, oldChild.GetNodeCountUpToNodeInPile(previousActiveNode, CardTraversal.CONTEXT))));
                }

            }
            else
            {
                // ------------- NEW CHILDREN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_tweenYFunc(newChild, newChild.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT)))
                    .Append(_tweenZFunc(newChild, _playSpaceBottomLeft.y))
                    .AppendInterval(_waitTime)
                    .Append(_tweenXFunc(newChild, newChild.Parent.Children.IndexOf(newChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                    .Append(_tweenYFunc(newChild, newChild.GetNodeCount(CardTraversal.CONTEXT))));
            }
        }

        // ------------- NEW MAIN ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_tweenYFunc(activeNode, activeNode.GetNodeCount(CardTraversal.CONTEXT)))
            .Append(_tweenZFunc(activeNode, _playSpaceBottomLeft.y))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_tweenYFunc(activeNode, 1)));


        if (discard != null)
        {
            _cardManager.AddToTopLevelMainPile(discard);
            _cardManager.AddToTopLevelMainPile(backToBe);

            // ------------- DISCARD ----------------

            int discardHeight = discard.GetNodeCount(CardTraversal.BODY);

            List<CardNode> lowerTopMostCardsRoot = backToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevelMainPile(node);
            }

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime + _horizontalTime)
                .Append(_tweenYFunc(discard, discardHeight)));

            // ------------- BackToBe ----------------


            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_tweenXFunc(backToBe, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_tweenYFunc(backToBe, backToBe.GetNodeCount(CardTraversal.BODY))));
        }
        else if (backToBe != null)
        {
            // ------------- BackToBe ----------------

            _cardManager.AddToTopLevelMainPile(backToBe);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_tweenXFunc(backToBe, _playSpaceBottomLeft.x)));
        }

        return entireSequence;
    }

}
