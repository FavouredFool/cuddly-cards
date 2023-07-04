
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

    public override async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (clickedNode.Context.GetCardType() == CardType.KEY || clickedNode.Context.GetCardType() == CardType.DIALOGUE)
        {
            // Close Up!
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
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
        // würde es sich hier lohnen den Hover auf eigene Klassen für jeden Kartentyp zu deligieren?
        CardType hoveredType = hoveredNode.Context.GetCardType();
        if (hoveredType == CardType.KEY || hoveredType == CardType.DIALOGUE)
        {
            // move upward certain amount -> Cardmover
        }
    }
}
