using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DialogueContext
{
    public DialogueContext(string name, string text)
    {
        Name = name;
        Text = text;
    }

    [OdinSerialize]
    public string Name { get; set; }
    [OdinSerialize]
    public string Text { get; set; }
}
