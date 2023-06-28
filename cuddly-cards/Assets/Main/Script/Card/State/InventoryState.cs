
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
        _animationManager.AddAnimation(CardInfo.CardTransition.TOINVENTORY);

        await _animationManager.PlayAnimations(_stateManager.GetCardManager().GetBaseNode(), true);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        // HERE NEEDS TO BE THE REFERENCE FOR THE ANIMATION FROM ANY STATE'S CLOSED TO OPEN

        _animationManager.AddAnimation(CardInfo.CardTransition.OPEN);
        _animationManager.AddAnimation(CardInfo.CardTransition.FROMINVENTORY);

        await _animationManager.PlayAnimations(_stateManager.GetCardManager().GetBaseNode(), true);

        _stateManager.PopState();
    }
}
