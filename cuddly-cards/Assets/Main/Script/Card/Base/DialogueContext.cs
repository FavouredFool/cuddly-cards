using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[Serializable]
public class DialogueContext
{
    public DialogueContext(bool isLockDialogue, string name, string text)
    {
        _isLockDialogue = isLockDialogue;
        _name = name;
        _text = text;
    }

    [TextArea(1, 2)]
    [SerializeField]
    string _name;
    [TextArea(3, 6)]
    [SerializeField]
    string _text;
    [SerializeField]
    bool _isLockDialogue;

    public bool IsLockDialogue { get { return _isLockDialogue; } set { _isLockDialogue = value; } }
    public string Name => _name;
    public string Text => _text;
}
