
using UnityEngine;
using static CardInfo;

public class MainState : LayoutState
{
    CardNode _baseNode;
    public MainState(CardManager cardManager, CardNode baseNode) :base (cardManager)
    {
        _baseNode = baseNode;
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _baseNode;
        _animationManager.SetCardsStatic();
    }

    public override async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        CardType cardType = clickedNode.Context.GetCardType();

        switch (cardType)
        {
            case CardType.INVENTORY:

                _animationManager.AddAnimation(CardTransition.CLOSE);
                _animationManager.AddAnimation(CardInfo.CardTransition.TOINVENTORY);
                await _animationManager.PlayAnimations(_cardManager.BaseNode);
                _stateManager.PushState(new InventoryState(_cardManager));

                return;

            case CardType.KEY:
            case CardType.DIALOGUE:

                if (!clickedNode.Context.GetHasBeenSeen())
                {
                    _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, true));
                    return;
                }

                _cardInventory.MoveNodeFromMainToInventory(clickedNode);

                return;

            case CardType.LOCK:

                if (!clickedNode.Context.GetHasBeenSeen())
                {
                    _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, true));
                    return;
                }

                _animationManager.AddAnimation(CardTransition.NOCHILDREN);
                _animationManager.AddAnimation(CardInfo.CardTransition.DISPLAYKEY);

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
        CardNode previousActiveNode = _baseNode;

        CardInfo.CardTransition cardTransition;
        LayoutState nextState;

        // closeUp
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, true));
            return;
        }
        else if (clickedNode == previousActiveNode)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
            return;
        }
        else if (previousActiveNode.Children.Contains(clickedNode))
        {
            // pressed child
            cardTransition = CardInfo.CardTransition.CHILD;
            nextState = new MainState(_cardManager, clickedNode);
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            // pressed back
            cardTransition = CardInfo.CardTransition.BACK;
            nextState = new MainState(_cardManager, clickedNode);
        }
        else if (clickedNode == rootNode)
        {
            // pressed root
            cardTransition = CardInfo.CardTransition.TOCOVER;
            nextState = new CoverState(_cardManager);
            _animationManager.AddAnimation(CardTransition.EXITINVENTORYPILE);
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        _animationManager.AddAnimation(cardTransition);

        await _animationManager.PlayAnimations(clickedNode, previousActiveNode);

        _stateManager.SetState(nextState);
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
