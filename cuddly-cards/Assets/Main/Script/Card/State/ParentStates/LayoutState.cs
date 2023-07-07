
using static CardInfo;

public abstract class LayoutState
{
    protected StateManager _stateManager;
    protected CardManager _cardManager;
    protected CloseUpManager _closeUpManager;
    protected CardMover _cardMover;
    protected AnimationManager _animationManager;
    protected CardInventory _cardInventory;

    protected LayoutState(CardManager cardManager)
    {
        _cardManager = cardManager;
        _stateManager = cardManager.StateManager;
        _closeUpManager = cardManager.CloseUpManager;
        _cardMover = cardManager.CardMover;
        _animationManager = cardManager.AnimationManager;
        _cardInventory = cardManager.CardInventory;
    }

    public abstract void StartState();
    public abstract void HandleClick(CardNode clickedNode, Click click);
    public abstract void HandleHover(CardNode hoveredNode);
}
