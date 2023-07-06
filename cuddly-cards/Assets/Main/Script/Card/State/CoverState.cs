
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CoverState : DefaultState
{
    public CoverState(CardManager cardManager, CardNode rootNode) : base(cardManager, rootNode)
    {
    }

    public override async void HandleIndividualTransitions(CardNode clickedNode)
    {
        _animationManager.AddAnimation(new FromCoverAnimation(_cardManager));
        _animationManager.AddAnimation(new EnterInventoryPileAnimation(_cardManager));
        await _animationManager.PlayAnimations(clickedNode);

        _stateManager.SetState(new MainState(_cardManager, clickedNode));
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
