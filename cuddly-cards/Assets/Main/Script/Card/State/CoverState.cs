
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

    public override async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null || clickedNode.Context.GetCardType() == CardType.INVENTORY)
        {
            // During the cover, the inventory can not be opened!
            return;
        }
        
        // --- Put the closeup state on the stack
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, true));
            return;
        }

        _animationManager.AddAnimation(CardInfo.CardTransition.FROMCOVER);
        _animationManager.AddAnimation(CardInfo.CardTransition.ENTERINVENTORYPILE);
        await _animationManager.PlayAnimations(clickedNode);

        _stateManager.SetState(new MainState(_cardManager, clickedNode));
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
