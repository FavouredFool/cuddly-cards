using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAnimation : CardAnimation
{
    public OpenAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode rootNode = _cardManager.RootNode;

        _cardManager.AddToTopLevelMainPile(baseNode);

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_horizontalTime)
            .Append(_subAnimations.RaiseNodeToHeight(baseNode, 1)));

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Children[i]);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveBaseToChild(baseNode.Children[i], baseNode.Children[i]))
                .Append(_subAnimations.LowerNodePile(baseNode.Children[i])));
        }

        return entireSequence;
    }
}
