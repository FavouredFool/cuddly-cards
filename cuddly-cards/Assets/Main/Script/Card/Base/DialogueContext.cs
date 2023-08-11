using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public struct DialogueContext
{
    public DialogueContext(bool isLockDialogue, string name, string text)
    {
        _isLockDialogue = isLockDialogue;
        _name = name;
        _text = text;
    }

    [SerializeField]
    bool _isLockDialogue;
    [SerializeField]
    string _name;
    [SerializeField]
    string _text;

    public bool IsLockDialogue => _isLockDialogue;
    public string Name => _name;
    public string Text => _text;
}
