using UnityEngine;

public class CloseUpState : LayoutState
{
    StateManager _manager;
    CloseUpManager _closeUpManager;
    CardNode _closeUpNode;
    Vector3 _originalPosition;
    Quaternion _originalRotation;
    bool _blockInputs;
    bool _clickAfterFinish;

    public CloseUpState(StateManager manager, CardNode clickedNode, bool clickAfterFinish)
    {
        _manager = manager;
        _closeUpNode = clickedNode;
        _closeUpManager = _manager.GetCloseUpManager();
        _clickAfterFinish = clickAfterFinish;
    }

    public async void StartState()
    {
        _closeUpNode.Context.SetHasBeenSeen(true);
        _originalPosition = _closeUpNode.Body.GetOriginalPosition();
        _originalRotation = _closeUpNode.Body.GetOriginalRotation();

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
        await _closeUpManager.RevertCloseUpAnimated(_closeUpNode, _originalPosition, _originalRotation);
        _blockInputs = false;

        _closeUpManager.RevertCloseUpStatic(_closeUpNode, _originalPosition, _originalRotation);

        _manager.PopState();

        if (_clickAfterFinish)
        {
            _manager.GetStates().Peek().HandleClick(_closeUpNode);
        }
    }

    public void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
