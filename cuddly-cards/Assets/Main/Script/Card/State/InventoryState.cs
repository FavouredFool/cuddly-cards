
public class InventoryState : LayoutState
{
    StateManager _manager;

    public InventoryState(StateManager manager)
    {
        _manager = manager;
    }

    public async void StartState()
    {
        //await _manager.GetCardManager().PrepareInventoryLayout();

        _manager.GetCardManager().FinishInventoryLayout();
    }

    public void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        // move back

        _manager.PopState();
    }
}
