
using UnityEngine;
using static CardInfo;

public class MainState : LayoutState
{
    StateManager _manager;
    CardNode _baseNode;

    public MainState(StateManager manager, CardNode baseNode)
    {
        _manager = manager;
        _baseNode = baseNode;
    }

    public void StartState()
    {
        _manager.GetCardManager().SetBaseNode(_baseNode);
        _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionStatic(_baseNode, CardTransition.CHILD);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (clickedNode.Context.GetCardType() == CardType.INVENTORY)
        {
            _manager.PushState(new InventoryState(_manager));

            await _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionAnimated(_baseNode, _baseNode, CardTransition.CLOSE);

            _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionStatic(_baseNode, CardTransition.CLOSE);
            return;
        }

        CardNode rootNode = _manager.GetCardManager().GetRootNode();
        CardNode previousActiveNode = _baseNode;

        CardInfo.CardTransition cardTransition;
        LayoutState nextState;

        // closeUp
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _manager.PushState(new CloseUpState(_manager, clickedNode, true));
            return;
        }
        else if (clickedNode == previousActiveNode)
        {
            _manager.PushState(new CloseUpState(_manager, clickedNode, false));
            return;
        }
        else if (previousActiveNode.Children.Contains(clickedNode))
        {
            // pressed child
            cardTransition = CardInfo.CardTransition.CHILD;
            nextState = new MainState(_manager, clickedNode);
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            // pressed back
            cardTransition = CardInfo.CardTransition.BACK;
            nextState = new MainState(_manager, clickedNode);
        }
        else if (clickedNode == rootNode)
        {
            // pressed root
            cardTransition = CardInfo.CardTransition.TOCOVER;
            nextState = new CoverState(_manager);
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        await _manager.GetCardManager().GetCardMover().SetLayoutBasedOnTransitionAnimated(clickedNode, previousActiveNode, cardTransition);
        _manager.SetState(nextState);
    }
}
