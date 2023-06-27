using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class MainLayout : SubLayout
{
    CardAnimation _enterInventoryPileAnimation;
    CardAnimation _exitInventoryPileAnimation;

    public MainLayout(List<CardAnimation> cardAnimations, CardManager cardManager) : base(cardAnimations, cardManager)
    {
        _enterInventoryPileAnimation = cardManager.GetCardMover().InstantiateEnterInventoryPileAnimation();
        _exitInventoryPileAnimation = cardManager.GetCardMover().InstantiateExitInventoryPileAnimation();
    }

    public override CardAnimation CardTransitionToAnimation(CardTransition transition)
    {
        switch (transition)
        {
            case CardTransition.CHILD:
                return _cardAnimations[0];
            case CardTransition.BACK:
                return _cardAnimations[1];
            case CardTransition.TOCOVER:
                return _cardAnimations[2];
            case CardTransition.FROMCOVER:
                return _cardAnimations[3];
            case CardTransition.CLOSE:
                return _cardAnimations[4];
            case CardTransition.OPEN:
                return _cardAnimations[5];
        }

        throw new System.Exception("CardAnimation not set");
    }

    public override void PrepareAnimation(CardTransition transition)
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        /*
        if (transition == CardTransition.TOCOVER)
            _exitInventoryPileAnimation.AnimateCards(null, null, null);
        else
            _enterInventoryPileAnimation.AnimateCards(null, null, null);
        */
    }

    public override void FinishAnimation(CardTransition transition)
    {
        
    }

    public override void PrepareStatic(CardTransition transition)
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        _cardMover.ResetPosition(_cardManager.GetRootNode());

        /*
        if (transition == CardTransition.TOCOVER)
            _exitInventoryPileAnimation.MoveCardsStatic(null, null);
        else
            _enterInventoryPileAnimation.MoveCardsStatic(null, null);

        _cardManager.GetCardMover().SetHeightOfInventory();
        _cardManager.GetCardMover().SetInventoryCardsRelativeToParent();
        */
    }

    public override void FinishStatic(CardTransition transition)
    {
        _cardMover.SetHeightOfTopLevelNodes();

        _cardMover.SetMainCardsRelativeToParent();

        _cardInput.SetColliders();
    }
}
