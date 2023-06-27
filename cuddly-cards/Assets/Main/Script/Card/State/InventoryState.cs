
public class InventoryState : LayoutState
{
    StateManager _manager;

    public InventoryState(StateManager manager)
    {
        _manager = manager;
    }

    public async void StartState()
    {
        _manager.GetCardManager().GetCardMover().AddAnimation(CardInfo.CardTransition.TOINVENTORY);

        await _manager.GetCardManager().GetCardMover().StartAnimations(_manager.GetCardManager().GetBaseNode(), true);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        // HERE NEEDS TO BE THE REFERENCE FOR THE ANIMATION FROM ANY STATE'S CLOSED TO OPEN

        _manager.GetCardManager().GetCardMover().AddAnimation(CardInfo.CardTransition.OPEN);
        _manager.GetCardManager().GetCardMover().AddAnimation(CardInfo.CardTransition.FROMINVENTORY);

        await _manager.GetCardManager().GetCardMover().StartAnimations(_manager.GetCardManager().GetBaseNode(), true);

        _manager.PopState();
    }
}
