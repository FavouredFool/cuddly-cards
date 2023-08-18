using System;
using UnityEngine;
using System.Collections.Generic;
using static CardManager;
using static CardInfo;

public class CardContext
{
    public CardContext(int cardID, CardType cardType, string label, string description, int desiredKeyID, int desiredTalkID, List<DialogueContext> dialogueContexts)
    {
        CardID = cardID;
        CardType = cardType;
        Label = label;
        Description = description;
        DesiredKeyID = desiredKeyID;
        DesiredTalkID = desiredTalkID;
        DialogueContexts = dialogueContexts;
    }

    public int CardID { get; private set; }
    public string Label { get; private set; }
    public string Description { get; private set; }
    public CardType CardType { get; private set; }
    public int DesiredKeyID { get; set; }
    public int DesiredTalkID { get; set; }
    public List<DialogueContext> DialogueContexts { get; set; }
}