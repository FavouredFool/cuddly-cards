
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
        if (clickedNode == null || clickedNode.Context.GetCardType() == CardInfo.CardType.INVENTORY)
        {
            return;
        }

        // closeUp
        _manager.PushState(new CloseUpState(_manager, clickedNode));
        // Closeup animation would be initialized here
        return;

        await _manager.GetCardManager().PrepareLayout(clickedNode, _manager.GetCardManager().GetActiveNode(), CardInfo.CardTransition.FROMCOVER);

        _manager.SetState(new MainState(_manager));
    }
}
