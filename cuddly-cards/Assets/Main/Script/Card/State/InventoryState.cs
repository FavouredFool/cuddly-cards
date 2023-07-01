
using UnityEngine;
using static CardInfo;

public class InventoryState : LayoutState
{
    StateManager _stateManager;
    AnimationManager _animationManager;

    public InventoryState(StateManager stateManager)
    {
        _stateManager = stateManager;
        _animationManager = stateManager.GetAnimationManager();
    }

    public async void StartState()
    {
        _animationManager.SetCardsStatic();
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (clickedNode.Context.GetCardType() == CardType.KEY || clickedNode.Context.GetCardType() == CardType.DIALOGUE)
        {
            // Close Up!
            _stateManager.PushState(new CloseUpState(_stateManager, clickedNode, false));
            return;
        }

        // HERE NEEDS TO BE THE REFERENCE FOR THE ANIMATION FROM ANY STATE'S CLOSED TO OPEN

        _animationManager.AddAnimation(CardInfo.CardTransition.OPEN);
        _animationManager.AddAnimation(CardInfo.CardTransition.FROMINVENTORY);

        await _animationManager.PlayAnimations(_stateManager.GetCardManager().GetBaseNode());

        _stateManager.PopState();
    }

    public void HandleHover(CardNode hoveredNode)
    {
        // würde es sich hier lohnen den Hover auf eigene Klassen für jeden Kartentyp zu deligieren?
        CardType hoveredType = hoveredNode.Context.GetCardType();
        if (hoveredType == CardType.KEY || hoveredType == CardType.DIALOGUE)
        {
            // move upward certain amount -> Cardmover
            _stateManager.GetCardManager().GetCardMover().HoverInventoryCards(hoveredNode);
        }
    }
}
