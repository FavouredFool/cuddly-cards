
using UnityEngine;
using static CardInfo;

public class MainState : LayoutState
{
    readonly CardNode _baseNode;
    public MainState(CardManager cardManager, CardNode baseNode) :base (cardManager)
    {
        _baseNode = baseNode;
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _baseNode;
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

        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.INVENTORY:

                _animationManager.AddAnimation(new CloseAnimation(_cardManager));
                _animationManager.AddAnimation(new ToInventoryAnimation(_cardManager));
                await _animationManager.PlayAnimations(_cardManager.BaseNode);
                _stateManager.PushState(new InventoryState(_cardManager));

                return;

            case CardType.KEY:
            case CardType.DIALOGUE:
                
                _cardInventory.MoveNodeFromMainToInventory(clickedNode);
                return;

            case CardType.LOCK:

                _animationManager.AddAnimation(new NoChildrenAnimation(_cardManager));
                _animationManager.AddAnimation(new DisplayKeysAnimation(_cardManager));

                await _animationManager.PlayAnimations(clickedNode, _baseNode);
                _stateManager.PushState(new LockState(_cardManager, clickedNode));
                return;

            default:

                EvaluateDefaultCardAction(clickedNode);

                return;
        }
    }

    public async void EvaluateDefaultCardAction(CardNode clickedNode)
    {
        CardNode rootNode = _cardManager.RootNode;

        LayoutState nextState;

        // closeUp
        if (clickedNode == _baseNode)
        {
            return;
        }
        if (_baseNode.Children.Contains(clickedNode))
        {
            // pressed child
            _animationManager.AddAnimation(new ChildAnimation(_cardManager));
            nextState = new MainState(_cardManager, clickedNode);
        }
        else if (_baseNode.Parent == clickedNode)
        {
            // pressed back
            _animationManager.AddAnimation(new BackAnimation(_cardManager));
            nextState = new MainState(_cardManager, clickedNode);
        }
        else if (clickedNode == rootNode)
        {
            // pressed root
            _animationManager.AddAnimation(new ToCoverAnimation(_cardManager));
            nextState = new CoverState(_cardManager);
            _animationManager.AddAnimation(new ExitInventoryPileAnimation(_cardManager));
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        await _animationManager.PlayAnimations(clickedNode, _baseNode);

        _stateManager.SetState(nextState);
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
