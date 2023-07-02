using UnityEngine;

public class CloseUpState : LayoutState
{
    CardNode _closeUpNode;
    Vector3 _originalPosition;
    Quaternion _originalRotation;
    bool _blockInputs;
    bool _clickAfterFinish;

    public CloseUpState(CardManager cardManager, CardNode clickedNode, bool clickAfterFinish) : base(cardManager)
    {
        _closeUpNode = clickedNode;
        _clickAfterFinish = clickAfterFinish;
    }

    public override async void StartState()
    {
        _closeUpNode.Context.SetHasBeenSeen(true);
        _originalPosition = _closeUpNode.Body.GetOriginalPosition();
        _originalRotation = _closeUpNode.Body.GetOriginalRotation();

        _blockInputs = true;
        await _closeUpManager.SetCloseUpAnimated(_closeUpNode);
        _blockInputs = false;

        _closeUpManager.SetCloseUpStatic(_closeUpNode);
    }

    public override async void HandleClick(CardNode clickedNode)
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

        _stateManager.PopState();

        if (_clickAfterFinish)
        {
            _stateManager.States.Peek().HandleClick(_closeUpNode);
        }
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
