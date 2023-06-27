
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
        _manager.GetCardManager().GetCardMover().AddAnimation(CardTransition.CHILD);
        _ = _manager.GetCardManager().GetCardMover().StartAnimations(_baseNode, false);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (clickedNode.Context.GetCardType() == CardType.INVENTORY)
        {
            _manager.GetCardManager().GetCardMover().AddAnimation(CardTransition.CLOSE);

            _manager.PushState(new InventoryState(_manager));
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

        _manager.GetCardManager().GetCardMover().AddAnimation(cardTransition);
        await _manager.GetCardManager().GetCardMover().StartAnimations(clickedNode, previousActiveNode, true);
        _manager.SetState(nextState);
    }
}
