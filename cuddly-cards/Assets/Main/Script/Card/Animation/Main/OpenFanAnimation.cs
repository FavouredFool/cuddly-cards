using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenFanAnimation : CardAnimation
{
    public OpenFanAnimation(CardManager cardManager) : base(cardManager) { }

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

        int count = baseNode.Children.Count;
        for (int i = 0; i < count; i++)
        {
            CardNode node = baseNode.Children[i];
            _cardManager.AddToTopLevelMainPile(baseNode.Children[i]);
            entireSequence.Join(_subAnimations.FanOutCards(node, i, count, false));
        }

        return entireSequence;
    }
}
