
using static CardInfo;

public abstract class DefaultState : LayoutState
{
    readonly CardNode _newBaseNode;

    protected DefaultState(CardManager cardManager, CardNode newBaseNode) : base(cardManager)
    {
        _newBaseNode = newBaseNode;
    }

    public override void HandleClick(CardNode clickedNode, Click click)
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

        HandleIndividualTransitions(clickedNode);
    }

    public abstract void HandleIndividualTransitions(CardNode clickedNode);

    public override void StartState()
    {
        _cardManager.BaseNode = _newBaseNode;
        SetStatic();
    }

    public void SetStatic()
    {
        _animationManager.SetCardsStatic();
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        // outlines pls?
        return;
    }
}
