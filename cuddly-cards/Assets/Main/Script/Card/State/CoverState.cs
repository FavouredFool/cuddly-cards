
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CoverState : LayoutState
{
    StateManager _manager;

    public CoverState(StateManager manager)
    {
        _manager = manager;
    }

    public void StartState()
    {
        _manager.GetCardManager().FinishLayout(CardTransition.TOCOVER);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null || clickedNode.Context.GetCardType() == CardType.INVENTORY)
        {
            // During the cover, the inventory can not be opened!
            return;
        }
        
        // --- Put the closeup state on the stack
        if (!clickedNode.Context.GetHasBeenSeen())
        {
            _manager.PushState(new CloseUpState(_manager, clickedNode, true));
            return;
        }

        await _manager.GetCardManager().PrepareLayout(clickedNode, _manager.GetCardManager().GetActiveNode(), CardInfo.CardTransition.FROMCOVER);

        _manager.SetState(new MainState(_manager));
    }
}
