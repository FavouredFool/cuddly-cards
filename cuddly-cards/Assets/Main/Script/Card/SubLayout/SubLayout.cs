using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public abstract class SubLayout
{
    protected List<CardAnimation> _cardAnimations;
    protected CardInput _cardInput;
    protected CardMover _cardMover;
    protected CardManager _cardManager;

    public SubLayout(List<CardAnimation> cardAnimations, CardManager cardManager)
    {
        _cardAnimations = cardAnimations;
        _cardManager = cardManager;
        _cardInput = _cardManager.GetCardInput();
        _cardMover = _cardManager.GetCardMover();
        
    }

    public abstract CardAnimation CardTransitionToAnimation(CardTransition transition);

    public abstract void PrepareAnimation(CardTransition transition);
    public abstract void FinishAnimation(CardTransition transition);

    public abstract void PrepareStatic(CardTransition transition);
    public abstract void FinishStatic(CardTransition transition);

}
