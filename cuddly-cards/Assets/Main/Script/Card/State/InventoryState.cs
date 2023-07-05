
using System.Linq;
using UnityEngine;
using static CardInfo;

public class InventoryState : LayoutState
{
    public InventoryState(CardManager cardManager) : base(cardManager)
    {
    }

    public override void StartState()
    {
        _animationManager.SetCardsStatic();
    }

    public override async void HandleClick(CardNode clickedNode, Click click)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (click == Click.RIGHT)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode));
            return;
        }

        if (clickedNode.Context.CardType is CardType.KEY or CardType.DIALOGUE)
        {
            return;
        }

        // HERE NEEDS TO BE THE REFERENCE FOR THE ANIMATION FROM ANY STATE'S CLOSED TO OPEN

        _animationManager.AddAnimation(new OpenAnimation(_cardManager));
        _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));

        await _animationManager.PlayAnimations(_cardManager.BaseNode);

        _stateManager.PopState();
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        ResetHovers(hoveredNode);

        if (hoveredNode == null)
        {
            return;
        }

        if (hoveredNode.Context.CardType is not (CardType.KEY or CardType.DIALOGUE)) return;

        if (hoveredNode.Body.IsHovered) return;

        hoveredNode.Body.StartHoverTween();
    }

    public void ResetHovers(CardNode hoveredNode)
    {
        foreach (CardNode childNode in _cardInventory.InventoryNode.Children.SelectMany(partNode => partNode.Children))
        {
            childNode.Body.ResetHover(hoveredNode);
        }
    }
}
