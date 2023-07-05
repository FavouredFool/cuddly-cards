
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CoverState : LayoutState
{
    public CoverState(CardManager cardManager) : base(cardManager)
    {
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _cardManager.RootNode;
        _animationManager.SetCardsStatic();
    }

    public override async void HandleClick(CardNode clickedNode, Click click)
    {
        if (clickedNode == null || clickedNode.Context.CardType == CardType.INVENTORY)
        {
            return;
        }

        if (click == Click.RIGHT)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode));
            return;
        }

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
