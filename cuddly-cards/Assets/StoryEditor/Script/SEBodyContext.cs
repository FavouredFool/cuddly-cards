using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using static CardManager;
using static CardInfo;

[ShowOdinSerializedPropertiesInInspector]
public class SEBodyContext : SerializedMonoBehaviour
{
    [BoxGroup("Basics")]
    [SerializeField]
    string _label;
    [BoxGroup("Basics")]
    [SerializeField]
    string _description;
    [BoxGroup("Basics")]
    [SerializeField]
    CardType _cardType;

    [ShowIf("_cardType", CardType.LOCK)]
    // Hier sicherstellen, dass der übergebene BodyContext "KEY" als Enum hat.
    [BoxGroup("Key")]
    [SerializeField]
    SEBodyContext _desiredKeyBody;

    [ShowIf("_cardType", CardType.DIALOGUE)]
    // Hier sicherstellen, dass der übergebene BodyContext "TALK" als Enum hat.
    [BoxGroup("Dialogue")]
    [SerializeField]
    SEBodyContext _talkBody;
    [ShowIf("_cardType", CardType.DIALOGUE)]
    [BoxGroup("Dialogue")]
    [SerializeField]
    List<DialogueContext> _dialogueContexts;

    public string Label { get { return _label; } set { _label = value; } }
    public string Description => _description;
    public CardType CardType => _cardType;
    public SEBodyContext DesiredKeyBody => _desiredKeyBody;
    public SEBodyContext TalkBody => _talkBody;
    public List<DialogueContext> DialogueContexts => _dialogueContexts;

    public void InitializeBodyContext(string label, string description, CardType cardType)
    {
        _label = label;
        _description = description;
        _cardType = cardType;
    }
}