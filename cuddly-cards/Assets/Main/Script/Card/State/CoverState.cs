
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CoverState : LayoutState
{
    StateManager _stateManager;
    CardNode _rootNode;
    AnimationManager _animationManager;

    public CoverState(StateManager manager)
    {
        _stateManager = manager;
        _rootNode = manager.GetCardManager().GetRootNode();
        _animationManager = _stateManager.GetAnimationManager();
    }

    public void StartState()
    {
        _stateManager.GetCardManager().SetBaseNode(_rootNode);
        _animationManager.SetCardsStatic();
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
            _stateManager.PushState(new CloseUpState(_stateManager, clickedNode, true));
            return;
        }

        _animationManager.AddAnimation(CardInfo.CardTransition.FROMCOVER);
        _animationManager.AddAnimation(CardInfo.CardTransition.ENTERINVENTORYPILE);
        await _animationManager.PlayAnimations(clickedNode);

        _stateManager.SetState(new MainState(_stateManager, clickedNode));
    }

    public void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
