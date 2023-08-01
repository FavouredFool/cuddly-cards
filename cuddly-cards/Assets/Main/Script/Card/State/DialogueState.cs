using UnityEngine;
using static CloseUpManager;

public class DialogueState : LayoutState
{
    readonly CardNode _closeUpNode;
    Vector3 _originalPosition;
    Quaternion _originalRotation;
    bool _blockInputs;

    public DialogueState(CardManager cardManager, CardNode clickedNode) : base(cardManager)
    {
        _closeUpNode = clickedNode;
    }

    public override async void StartState()
    {
        Transform transform = _closeUpNode.Body.transform;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;

        _blockInputs = true;
        await _closeUpManager.SetCloseUpAnimated(_closeUpNode, CloseUpStyle.DIALOGUE);
        _blockInputs = false;

        _closeUpManager.SetCloseUpStatic(_closeUpNode, CloseUpStyle.DIALOGUE);
        _closeUpManager.SetText("Somethign something dialogue");
    }

    public override void HandleClick(CardNode clickedNode, CardInfo.Click click)
    {
        // DONT USE CLICKEDNODE; IT DOESNT WORK, WHICH IS INTENTIONAL. USE _CLOSEUPNODE

        if (_blockInputs)
        {
            return;
        }

        if (click == CardInfo.Click.RIGHT)
        {
            RightClick();
        }
        else if (click == CardInfo.Click.LEFT)
        {
            LeftClick();
        }
    }

    public void LeftClick()
    {
        _ = _closeUpManager.Flip(_closeUpNode);
        _closeUpManager.SetText("different text");
    }

    public async void RightClick()
    {
        _blockInputs = true;
        await _closeUpManager.RevertCloseUpAnimated(_closeUpNode, _originalPosition, _originalRotation, CloseUpStyle.DIALOGUE);
        _blockInputs = false;

        _closeUpManager.RevertCloseUpStatic(_closeUpNode, _originalPosition, _originalRotation, CloseUpStyle.DIALOGUE);

        _stateManager.PopState();
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
