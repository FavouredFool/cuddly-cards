
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class DialogueLockState : SettedState
{
    DialogueState _dialogueState;

    public DialogueLockState(CardManager cardManager, CardNode baseNode, DialogueState dialogueState) : base (cardManager, baseNode)
    {
        _dialogueState = dialogueState;
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.INVENTORY:
                return;

            case CardType.KEY:
                PlayNode(clickedNode);
                return;

            case CardType.DIALOGUE:
            case CardType.COVER:
            case CardType.PLACE:
            case CardType.THING:
            case CardType.PERSON:
            case CardType.LOCK:
            default:
                ToDefaultTransitions(clickedNode);
                return;
        }
    }

    void ToDefaultTransitions(CardNode clickedNode)
    {
        if (clickedNode == _cardManager.BaseNode) return;

        if (_cardManager.BaseNode.Parent == clickedNode)
        {
            _dialogueState.SetDialogueCondition(DialogueState.DialogueCondition.UPCLOSE);

            ToBackTransition(clickedNode);
            return;
        }

        if (clickedNode == _cardManager.RootNode)
        {
            ToRootTransition(clickedNode);
            return;
        }
    }

    void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ToCoverAnimation(_cardManager), new FromInventoryAnimation(_cardManager, false), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    void ToBackTransition(CardNode clickedNode)
    {
        // Transition back to the dialogue to the previously active position.

        List<CardAnimation> animations = new() { new BackAnimation(_cardManager, true, false), new FromInventoryAnimation(_cardManager, true) };

        PopTransition(clickedNode, animations);
    }

    public async void PopTransition(CardNode clickedNode, List<CardAnimation> animations)
    {
        foreach (CardAnimation animation in animations)
        {
            _animationManager.AddAnimation(animation);
        }

        await _animationManager.PlayAnimations(clickedNode, _cardManager.BaseNode);

        _stateManager.PopState();
    }

    void PlayNode(CardNode clickedNode)
    {
        CardNode baseNode = _cardManager.BaseNode;

        if (baseNode.Context.DesiredKeyID.Equals(clickedNode.Context.CardID))
        {
            NodeCorrect(clickedNode, baseNode);
        }
        else
        {
            NodeWrong();
        }
    }

    void NodeWrong()
    {
        Debug.Log("WRONG!");
    }

    async void NodeCorrect(CardNode clickedNode, CardNode baseNode)
    {
        await LockOpened(baseNode, clickedNode);

        CardNode dialogueNode = baseNode.Parent;

        // Für die Animation muss ich das im Voraus machen, bevor es erneut von dem MainState gesetzt wird
        _cardManager.BaseNode = dialogueNode;

        List<CardAnimation> animations = new() { new FromInventoryAnimation(_cardManager, false), new OpenAnimation(_cardManager) };

        _dialogueState.SetDialogueCondition(DialogueState.DialogueCondition.UPCLOSE);
        _dialogueState.IncreaseDialogueStartPosition();

        PopTransition(dialogueNode, animations);
    }

    public async Task LockOpened(CardNode lockNode, CardNode keyNode)
    {
        RemoveKeyFromTree(keyNode);
        RemoveLockFromTree(lockNode);

        EndHover(lockNode);
        EndHover(keyNode);

        _ = DisintegrateCard(lockNode);
        await DisintegrateCard(keyNode);
        
        Object.Destroy(keyNode.Body.gameObject);
        Object.Destroy(lockNode.Body.gameObject);
    }

    public void RemoveLockFromTree(CardNode lockNode)
    {
        lockNode.Parent.Children.Remove(lockNode);
    }

    public void RemoveKeyFromTree(CardNode keyNode)
    {
        keyNode.Parent.Children.Remove(keyNode);
    }

    public async Task DisintegrateCard(CardNode node)
    {
        await node.Body.DisintegrateCard();
    }

    public override void StartHover(CardNode hoveredNode)
    {
        hoveredNode.Body.StartOutline();

        if (hoveredNode.Context.CardType is not (CardType.KEY)) return;

        hoveredNode.Body.StartHoverTween();
    }

    public override void EndHover(CardNode hoveredNode)
    {
        hoveredNode.Body.EndHoverTween();
        hoveredNode.Body.EndOutline();
    }
}
