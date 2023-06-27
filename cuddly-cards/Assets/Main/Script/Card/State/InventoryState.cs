
public class InventoryState : LayoutState
{
    StateManager _manager;

    public InventoryState(StateManager manager)
    {
        _manager = manager;
    }

    public async void StartState()
    {
        await _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionAnimated(null, null, CardInfo.CardTransition.TOINVENTORY);

        _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionStatic(null, CardInfo.CardTransition.TOINVENTORY);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        // HERE NEEDS TO BE THE REFERENCE FOR THE ANIMATION FROM ANY STATE'S CLOSED TO OPEN

        _ = _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionAnimated(_manager.GetCardManager().GetBaseNode(), null, CardInfo.CardTransition.OPEN);

        await _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionAnimated(null, null, CardInfo.CardTransition.FROMINVENTORY);

        _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionStatic(_manager.GetCardManager().GetBaseNode(), CardInfo.CardTransition.OPEN);

        _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionStatic(null, CardInfo.CardTransition.FROMINVENTORY);

        _manager.PopState();
    }
}
