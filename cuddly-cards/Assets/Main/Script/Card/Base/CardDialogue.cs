using System.Collections.Generic;
using UnityEngine;

public class CardDialogue
{
    private CardManager _cardManager;
    public CardDialogue(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void SpreadDialogues(CardNode dialogueWrapperNode)
    {
        AnimationManager animationManager = _cardManager.AnimationManager;
        List<CardNode> dialogueNodes = new();

        dialogueWrapperNode.UnlinkFromParent();

        foreach (CardNode node in dialogueWrapperNode.Children)
        {
            dialogueNodes.Add(node);
        }

        foreach (CardNode node in dialogueNodes)
        {
            node.UnlinkFromParent();
        }

        // Animate cards moving into their intended positions.

        // kill wrapperBody (the node in memory will be garbage collected)
        GameObject.Destroy(dialogueWrapperNode.Body.gameObject);

        // relink nodes, so that they can be properly positioned statically.
            // How do I find out to which cards this Dialogue card is linked - and at which exact position? Dialogue-Card side or other cards and you iterate through them all (definitely the first one)

        foreach (CardNode node in dialogueNodes)
        {
            CardNode talkParentNode = _cardManager.GetCardNodeFromID(node.Context.TalkID);
            talkParentNode.AddChild(node);
        }


        // hit up manual static recalculation.
        animationManager.SetCardsStatic();
    }
}
