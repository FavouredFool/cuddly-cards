
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;

public abstract class CardAnimation
{
    protected CardManager _cardManager;
    protected CardMover _cardMover;
    protected SubAnimations _subAnimations;
    protected SoundManager _soundManager;

    protected float _waitTime;
    protected float _horizontalTime;
    protected float _verticalTime;

    protected CardAnimation(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.CardMover;
        _subAnimations = _cardMover.SubAnimations;
        _soundManager = cardManager.SoundManager;

        _waitTime = _cardMover.WaitTime;
        _horizontalTime = _cardMover.HorizontalTime;
        _verticalTime = _cardMover.VerticalTime;
    }

    public abstract Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode);
}
