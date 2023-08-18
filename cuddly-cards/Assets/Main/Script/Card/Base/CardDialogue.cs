using System.Collections.Generic;
using UnityEngine;

public class CardDialogue
{
    private CardManager _cardManager;
    private CloseUpManager _closeUpManager;
    public CardDialogue(CardManager cardManager)
    {
        _cardManager = cardManager;
        _closeUpManager = cardManager.CloseUpManager;
    }
    public async void SpreadDialogues(CardNode dialogueWrapperNode)
    {
        AnimationManager animationManager = _cardManager.AnimationManager;
        List<CardNode> dialogueNodes = new();

        animationManager.AddAnimation(new SpreadDialogueAnimationPart1(_cardManager));
        await animationManager.PlayAnimations(dialogueWrapperNode, _cardManager.BaseNode);

        // Unlink Wrapper
        dialogueWrapperNode.UnlinkFromParent();

        foreach (CardNode node in dialogueWrapperNode.Children)
        {
            dialogueNodes.Add(node);
        }

        foreach (CardNode node in dialogueNodes)
        {
            node.UnlinkFromParent();
        }

        // Dissolve effect!
        await dialogueWrapperNode.Body.DisintegrateCard();
        Object.Destroy(dialogueWrapperNode.Body.gameObject);

        foreach (CardNode node in dialogueNodes)
        {
            animationManager.AddAnimation(new SpreadDialogueAnimationPart2(_cardManager));
            await animationManager.PlayAnimations(node, _cardManager.BaseNode);

            CardNode talkParentNode = _cardManager.GetCardNodeFromID(node.Context.DesiredTalkID);
            talkParentNode.AddChild(node);
        }

        animationManager.AddAnimation(new SpreadDialogueAnimationPart3(_cardManager));
        await animationManager.PlayAnimations(_cardManager.BaseNode, _cardManager.BaseNode);

        // Reset Position
        animationManager.SetCardsStatic();
    }

}
