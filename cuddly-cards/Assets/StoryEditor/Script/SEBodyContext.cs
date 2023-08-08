using System;
using UnityEngine;
using System.Collections.Generic;
using static CardManager;
using static CardInfo;

public class SEBodyContext : MonoBehaviour
{
    [SerializeField] int _id;
    [SerializeField] string _label;
    [SerializeField] string _description;
    [SerializeField] CardType _cardType;
    [SerializeField] string _desiredKey;
    [SerializeField] int _talkID;
    [SerializeField] List<DialogueContext> _dialogueContexts;

    public int ID => _id;
    public string Label { get { return _label; } set { _label = value; } }
    public string Description => _description;
    public CardType CardType => _cardType;
    public string DesiredKey => _desiredKey;
    public int TalkID => _talkID;
    public List<DialogueContext> DialogueContexts => _dialogueContexts;

    public void InitializeBodyContext(int id, string label, string description, CardType cardType, string desiredKey, int talkID, List<DialogueContext> dialogueContexts)
    {
        _id = id;
        _label = label;
        _description = description;
        _cardType = cardType;
        _desiredKey = desiredKey;
        _talkID = talkID;
        _dialogueContexts = dialogueContexts;
    }
}