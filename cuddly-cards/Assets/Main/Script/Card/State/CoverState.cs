
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CoverState : SettedState
{
    public CoverState(CardManager cardManager) : base(cardManager, cardManager.RootNode)
    {
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new FromCoverAnimation(_cardManager), new EnterInventoryPileAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }
}
