
using System.Threading.Tasks;
using UnityEngine;

public class CoverState : LayoutState
{
    StateManager _manager;

    public CoverState(StateManager manager)
    {
        _manager = manager;
    }

    public void StartState()
    {
        _manager.GetCardManager().FinishLayout(true);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        if (clickedNode.Context.GetCardType() == CardInfo.CardType.INVENTORY)
        {
            return;
        }

        await _manager.GetCardManager().PrepareLayout(clickedNode, _manager.GetCardManager().GetActiveNode(), CardInfo.CardTransition.FROMCOVER);

        _manager.SetState(new MainState(_manager));
    }
}
