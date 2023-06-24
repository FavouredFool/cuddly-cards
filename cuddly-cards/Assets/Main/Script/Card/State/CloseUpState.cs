using UnityEngine;

public class CloseUpState : LayoutState
{
    StateManager _manager;
    CloseUpManager _closeUpManager;
    CardNode _closeUpNode;
    Vector3 _originalPosition;
    bool _blockInputs;

    public CloseUpState(StateManager manager, CardNode clickedNode)
    {
        _manager = manager;
        _closeUpNode = clickedNode;
        _closeUpManager = _manager.GetCloseUpManager();
    }

    public async void StartState()
    {
        _closeUpNode.Context.SetHasBeenSeen(true);
        _originalPosition = _closeUpNode.Body.transform.position;

        _blockInputs = true;
        await _closeUpManager.SetCloseUpAnimated(_closeUpNode);
        _blockInputs = false;

        _closeUpManager.SetCloseUpStatic(_closeUpNode);
    }

    public async void HandleClick(CardNode clickedNode)
    {
        // DONT USE CLICKEDNODE -> IT DOESNT WORK, WHICH IS INTENTIONAL. USE _CLOSEUPNODE

        if (_blockInputs)
        {
            return;
        }
        
        _blockInputs = true;
        await _closeUpManager.RevertCloseUpAnimated(_closeUpNode, _originalPosition);
        _blockInputs = false;

        _closeUpManager.RevertCloseUpStatic(_closeUpNode, _originalPosition);

        _manager.PopState();

        _manager.GetStates().Peek().HandleClick(_closeUpNode);
    }
}
