
using UnityEngine;
using static CardInfo;

public class LockState : LayoutState
{
    CardNode _baseNode;

    public LockState(CardManager cardManager, CardNode baseNode) : base (cardManager)
    {
        _baseNode = baseNode;
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _baseNode;
        _animationManager.SetCardsStatic();
    }

    public override void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        CardType cardType = clickedNode.Context.GetCardType();

        switch (cardType)
        {
            case CardType.INVENTORY:
                return;

            case CardType.KEY:
            case CardType.DIALOGUE:
                _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
                return;

            default:
                EvaluateDefaultCardAction(clickedNode);
                return;
        }
    }

    public async void EvaluateDefaultCardAction(CardNode clickedNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        CardNode previousActiveNode = _cardManager.BaseNode;

        LayoutState nextState;

        if (clickedNode == previousActiveNode)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
            return;
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            _animationManager.AddAnimation(new BackAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));

            nextState = new MainState(_cardManager, clickedNode);

        }
        else if (clickedNode == rootNode)
        {
            _animationManager.AddAnimation(new ToCoverAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));
            _animationManager.AddAnimation(new ExitInventoryPileAnimation(_cardManager));

            nextState = new CoverState(_cardManager);
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        await _animationManager.PlayAnimations(clickedNode, previousActiveNode);

        _stateManager.SetState(nextState);

    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
