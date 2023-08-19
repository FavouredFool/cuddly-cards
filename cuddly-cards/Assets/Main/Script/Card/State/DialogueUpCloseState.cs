using UnityEngine;
using System.Collections.Generic;
using static CloseUpManager;

public class DialogueUpCloseState : LayoutState
{
    CardNode _closeUpNode;
    CardBuilder _cardBuilder;
    Vector3 _originalPosition;
    Quaternion _originalRotation;
    bool _blockInputs;
    int _dialogueIterator = 0;

    DialogueState _dialogueState;

    public DialogueUpCloseState(CardManager cardManager, CardNode clickedNode, DialogueState dialogueState) : base(cardManager)
    {
        _closeUpNode = clickedNode;
        _cardBuilder = cardManager.CardBuilder;
        _dialogueState = dialogueState;
    }

    public override async void StartState()
    {
        Transform transform = _closeUpNode.Body.transform;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;

        DialogueContext dialogueContext = _closeUpNode.Context.DialogueContexts[0];

        _blockInputs = true;
        await _closeUpManager.SetCloseUpAnimated(_closeUpNode, CloseUpStyle.DIALOGUE, _cardManager, dialogueContext, true);
        _blockInputs = false;

        _closeUpManager.SetCloseUpStatic(_closeUpNode, CloseUpStyle.DIALOGUE);
        _closeUpManager.SetText(dialogueContext.Text);
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
        _dialogueIterator -= 1;

        if (_dialogueIterator < 0)
        {
            EndDialogue(false);
            return;
        }

        SetUpDialogue(_dialogueIterator, false);
    }

    public void RightClick()
    {
        _dialogueIterator += 1;

        if (_dialogueIterator >= _closeUpNode.Context.DialogueContexts.Count)
        {
            EndDialogue(true);
            return;
        }

        SetUpDialogue(_dialogueIterator, true);
    }

    public async void SetUpDialogue(int index, bool flipRight)
    {
        DialogueContext dialogueContext = _closeUpNode.Context.DialogueContexts[index];

        _closeUpManager.SetText("");
        _blockInputs = true;
        await _closeUpManager.Flip(_closeUpNode, dialogueContext.Name, _cardBuilder.GetPersonImageFromCard(), flipRight);
        _blockInputs = false;

        _closeUpManager.SetText(dialogueContext.Text);
    }

    public async void EndDialogue(bool flipRight)
    {
        _blockInputs = true;
        await _closeUpManager.RevertCloseUpAnimated(_closeUpNode, _originalPosition, _originalRotation, CloseUpStyle.DIALOGUE, _cardManager, flipRight);
        _blockInputs = false;

        _closeUpManager.RevertCloseUpStatic(_closeUpNode, _originalPosition, _originalRotation, CloseUpStyle.DIALOGUE);

        if (flipRight)
        {
            _dialogueState.SetDialogueCondition(DialogueState.DialogueCondition.ACCEPTED);
        }
        else
        {
            _dialogueState.SetDialogueCondition(DialogueState.DialogueCondition.REJECTED);
        }
        

        _stateManager.PopState();
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
