using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseAnimation : CardAnimation
{
    public CloseAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevelMainPile(baseNode);

        entireSequence.Join(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode));

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        foreach (CardNode childNode in baseNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(childNode, baseNode))
                .Append(_subAnimations.MoveNodeXToLeft(childNode)));
        }

        return entireSequence;
    }
}
