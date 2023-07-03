using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAnimation : CardAnimation
{
    public OpenAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode rootNode = _cardManager.RootNode;

        _cardManager.AddToTopLevelMainPile(activeNode);

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_horizontalTime)
            .Append(_subAnimations.RaiseNodeToHeight(activeNode, 1)));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        for (int i = 0; i < activeNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Children[i]);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveBaseToChild(activeNode.Children[i], activeNode.Children[i]))
                .Append(_subAnimations.LowerNodePile(activeNode.Children[i])));
        }

        return entireSequence;
    }
}
