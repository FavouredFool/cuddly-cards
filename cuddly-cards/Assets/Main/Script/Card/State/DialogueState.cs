
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardInfo;

public class DialogueState : SettedState
{
    bool _initialDialogueStart;
    public DialogueState(CardManager cardManager, CardNode newBaseNode) : base(cardManager, newBaseNode)
    {
        _initialDialogueStart = true;
    }

    public override void StartState()
    {
        if (_initialDialogueStart)
        {
            _cardManager.BaseNode = _newBaseNode;
            SetStatic();

            _stateManager.PushState(new DialogueUpCloseState(_cardManager, _newBaseNode));
            _initialDialogueStart = false;
        }
        else
        {
            ToTalkTransition(_newBaseNode.Parent);
        }
        
    }

    public void ToTalkTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager, false, true) };
        LayoutState newState = new TalkState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {

    }

    public override void StartHover(CardNode hoveredNode)
    {
        
    }

    public override void EndHover(CardNode hoveredNode)
    {
        
    }
}
