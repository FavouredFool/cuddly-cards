using System.Collections.Generic;
using UnityEngine;

public class CardDialogue
{
    private CardManager _cardManager;
    public CardDialogue(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void SpreadDialogues(CardNode parentDialogueNode)
    {
        Debug.LogWarning("Not implemented");

        return;
        AnimationManager animationManager = _cardManager.AnimationManager;

        List<CardNode> clickedDialogues = new() { parentDialogueNode };

        foreach (CardNode node in parentDialogueNode.Children)
        {
            clickedDialogues.Add(node);
        }

        foreach (CardNode node in clickedDialogues)
        {
            node.UnlinkFromParent();
        }

        // Animate cards moving into their intended positions.

        // relink them, so that they can be properly positioned statically.
            // How do I find out to which cards this Dialogue card is linked - and at which exact position? Dialogue-Card side or other cards and you iterate through them all (definitely the first one)

        // hit up manual static recalculation.
        animationManager.SetCardsStatic();
    }
}
