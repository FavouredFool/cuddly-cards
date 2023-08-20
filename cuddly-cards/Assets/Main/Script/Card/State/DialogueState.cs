
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardInfo;

public class DialogueState : SettedState
{

    public enum DialogueCondition { UPCLOSE, REJECTED, ACCEPTED, LOCKED }

    DialogueCondition _dialogueCondition = DialogueCondition.UPCLOSE;
    int _dialogueStartPosition = 0;
    int _lockAmount = -1;

    public DialogueState(CardManager cardManager, CardNode newBaseNode) : base(cardManager, newBaseNode)
    {

    }

    public override void StartState()
    {
        _cardManager.BaseNode = _newBaseNode;

        // Animations?

        SetStatic();

        switch (_dialogueCondition)
        {
            case DialogueCondition.UPCLOSE:
                    _stateManager.PushState(new DialogueUpCloseState(_cardManager, _newBaseNode, _dialogueStartPosition, this));
                break;
            case DialogueCondition.LOCKED:
                ToDialogueLockTransition(_cardManager.BaseNode.Children[_lockAmount]);
                break;
            case DialogueCondition.REJECTED:
                    ToTalkTransition(_newBaseNode.Parent);
                break;
            case DialogueCondition.ACCEPTED:
                    SetStatic();
                break;
        }
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.DWRAPPER:
                CollectDialogue(clickedNode);
                return;

            case CardType.KEY:
                CollectKey(clickedNode);
                return;

            case CardType.COVER:
            case CardType.PLACE:
            case CardType.THING:
            case CardType.PERSON:
            default:
                ToDefaultTransitions(clickedNode);
                return;
        }
    }

    public void CollectKey(CardNode clickedNode)
    {
        _cardInventory.MoveKeyFromMainToInventory(clickedNode);
    }

    public void CollectDialogue(CardNode clickedNode)
    {
        _cardDialogue.SpreadDialogues(clickedNode);
    }

    public void ToDialogueLockTransition(CardNode lockNode)
    {
        List<CardAnimation> animations = new() { new NoChildrenAnimation(_cardManager), new ToInventoryAnimation(_cardManager, false) };
        LayoutState newState = new DialogueLockState(_cardManager, lockNode, this);

        PushTransition(lockNode, animations, newState);
    }

    public async void PushTransition(CardNode clickedNode, List<CardAnimation> animations, LayoutState state)
    {
        foreach (CardAnimation animation in animations)
        {
            _animationManager.AddAnimation(animation);
        }

        await _animationManager.PlayAnimations(clickedNode, _cardManager.BaseNode);

        _stateManager.PushState(state);
    }

    public void ToDefaultTransitions(CardNode clickedNode)
    {
        if (clickedNode == _cardManager.BaseNode) return;

        if (_cardManager.BaseNode.Parent == clickedNode)
        {
            ToBackTransition(clickedNode);
            return;
        }

        if (clickedNode == _cardManager.RootNode)
        {
            ToRootTransition(clickedNode);
            return;
        }
    }

    public void ToChildTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ChildAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager, false, false) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ToCoverAnimation(_cardManager), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToTalkTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager, false, true) };
        LayoutState newState = new TalkState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public override void StartHover(CardNode hoveredNode)
    {
        
    }

    public override void EndHover(CardNode hoveredNode)
    {
        
    }

    public void SetDialogueCondition(DialogueCondition condition)
    {
        _dialogueCondition = condition;
    }

    public DialogueCondition GetDialogueCondition()
    {
        return _dialogueCondition;
    }

    public void SetDialogueStartPosition(int dialogueStartPosition)
    {
        _dialogueStartPosition = dialogueStartPosition;
    }

    public void IncreaseDialogueStartPosition()
    {
        _dialogueStartPosition++;
    }

    public void IncreaseLockAmount()
    {
        _lockAmount++;
    }
}
