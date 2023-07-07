using UnityEngine;

public class CloseUpState : LayoutState
{
    readonly CardNode _closeUpNode;
    Vector3 _originalPosition;
    Quaternion _originalRotation;
    bool _blockInputs;

    public CloseUpState(CardManager cardManager, CardNode clickedNode) : base(cardManager)
    {
        _closeUpNode = clickedNode;
    }

    public override async void StartState()
    {
        Transform transform = _closeUpNode.Body.transform;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;

        _blockInputs = true;
        await _closeUpManager.SetCloseUpAnimated(_closeUpNode);
        _blockInputs = false;

        _closeUpManager.SetCloseUpStatic(_closeUpNode);
    }

    public override async void HandleClick(CardNode clickedNode, CardInfo.Click click)
    {
        // DONT USE CLICKEDNODE; IT DOESNT WORK, WHICH IS INTENTIONAL. USE _CLOSEUPNODE

        if (_blockInputs)
        {
            return;
        }
        
        _blockInputs = true;
        await _closeUpManager.RevertCloseUpAnimated(_closeUpNode, _originalPosition, _originalRotation);
        _blockInputs = false;

        _closeUpManager.RevertCloseUpStatic(_closeUpNode, _originalPosition, _originalRotation);

        _stateManager.PopState();
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
