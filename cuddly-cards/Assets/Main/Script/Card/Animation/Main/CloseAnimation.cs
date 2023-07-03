using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseAnimation : CardAnimation
{
    public CloseAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevelMainPile(activeNode);

        entireSequence.Join(_subAnimations.LiftNodePile(activeNode));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        foreach (CardNode childNode in activeNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.RaiseNodePileRelative(childNode, activeNode))
                .Append(_subAnimations.MoveNodeToLeft(childNode)));
        }

        return entireSequence;
    }
}
