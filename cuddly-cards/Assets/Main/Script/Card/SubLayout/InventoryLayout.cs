using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class InventoryLayout : SubLayout
{
    public InventoryLayout(List<CardAnimation> cardAnimations, CardManager cardManager) : base(cardAnimations, cardManager) {}

    public override CardAnimation CardTransitionToAnimation(CardTransition transition)
    {
        switch (transition)
        {
            case CardTransition.TOINVENTORY:
                return _cardAnimations[0];
            case CardTransition.FROMINVENTORY:
                return _cardAnimations[1];
        }

        throw new System.Exception("CardAnimation not set");
    }

    public override void PrepareAnimation(CardTransition transition)
    {
        _cardInput.RemoveColliders();
    }

    public override void FinishAnimation(CardTransition transition)
    {
        
    }

    public override void PrepareStatic(CardTransition transition)
    {
        _cardInput.RemoveColliders();
    }

    public override void FinishStatic(CardTransition transition)
    {
        _cardMover.SetHeightAndRotationOfInventory();

        _cardMover.SetInventoryCardsRelativeToParent();

        _cardInput.SetColliders();
    }
}
