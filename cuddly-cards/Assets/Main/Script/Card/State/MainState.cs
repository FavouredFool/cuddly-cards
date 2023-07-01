
using UnityEngine;
using static CardInfo;

public class MainState : LayoutState
{
    StateManager _stateManager;
    CardNode _baseNode;
    AnimationManager _animationManager;

    public MainState(StateManager manager, CardNode baseNode)
    {
        _stateManager = manager;
        _baseNode = baseNode;
        _animationManager = _stateManager.GetAnimationManager();
    }

    public void StartState()
    {
        _stateManager.GetCardManager().SetBaseNode(_baseNode);
        _animationManager.SetCardsStatic();
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (clickedNode.Context.GetCardType() == CardType.INVENTORY)
        {
            _animationManager.AddAnimation(CardTransition.CLOSE);

            _animationManager.AddAnimation(CardInfo.CardTransition.TOINVENTORY);

            await _animationManager.PlayAnimations(_stateManager.GetCardManager().GetBaseNode());

            _stateManager.PushState(new InventoryState(_stateManager));
            return;
        }

        CardNode rootNode = _stateManager.GetCardManager().GetRootNode();
        CardNode previousActiveNode = _baseNode;

        CardInfo.CardTransition cardTransition;
        LayoutState nextState;

        // closeUp
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _stateManager.PushState(new CloseUpState(_stateManager, clickedNode, true));
            return;
        }
        else if (clickedNode == previousActiveNode)
        {
            _stateManager.PushState(new CloseUpState(_stateManager, clickedNode, false));
            return;
        }
        else if (previousActiveNode.Children.Contains(clickedNode))
        {
            // pressed child
            cardTransition = CardInfo.CardTransition.CHILD;
            nextState = new MainState(_stateManager, clickedNode);
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            // pressed back
            cardTransition = CardInfo.CardTransition.BACK;
            nextState = new MainState(_stateManager, clickedNode);
        }
        else if (clickedNode == rootNode)
        {
            // pressed root
            cardTransition = CardInfo.CardTransition.TOCOVER;
            nextState = new CoverState(_stateManager);
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

    public void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
