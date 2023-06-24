
using UnityEngine;
using static CardInfo;

public class MainState : LayoutState
{
    StateManager _manager;

    public MainState(StateManager manager)
    {
        _manager = manager;
    }

    public void StartState()
    {
        _manager.GetCardManager().FinishLayout(CardTransition.CHILD);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null || clickedNode.Context.GetCardType() == CardInfo.CardType.INVENTORY)
        {
            return;
        }

        CardNode rootNode = _manager.GetCardManager().GetRootNode();
        CardNode previousActiveNode = _manager.GetCardManager().GetActiveNode();

        CardInfo.CardTransition cardTransition;
        LayoutState nextState;

        // closeUp
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _manager.PushState(new CloseUpState(_manager, clickedNode));
            return;
        }

        if (clickedNode == previousActiveNode)
        {
            // closeup without layoutchange
            return;
        }
        else if (previousActiveNode.Children.Contains(clickedNode))
        {
            // pressed child
            cardTransition = CardInfo.CardTransition.CHILD;
            nextState = new MainState(_manager);
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            // pressed back
            cardTransition = CardInfo.CardTransition.BACK;
            nextState = new MainState(_manager);
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

        await _manager.GetCardManager().PrepareLayout(clickedNode, previousActiveNode, cardTransition);
        _manager.SetState(nextState);
    }
}
