using UnityEngine;
using TMPro;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using static CardInfo;

public class SEBody : MonoBehaviour
{
    [PropertyOrder(1)]
    [BoxGroup("Basics")]
    public CardType CardType;

    [PropertyOrder(1)]
    [BoxGroup("Basics")]
    [MultiLineProperty(2)]
    public string Label;

    [PropertyOrder(1)]
    [BoxGroup("Basics")]
    [MultiLineProperty(5)]
    public string Description;

    [ShowIf("CardType", CardType.LOCK)]
    [PropertyOrder(2)]
    [BoxGroup("Key")]
    public SEBody DesiredKey;

    [ShowIf("CardType", CardType.DIALOGUE)]
    [PropertyOrder(3)]
    [BoxGroup("Dialogue")]
    public SEBody DesiredTalk;

    [ShowIf("CardType", CardType.DIALOGUE)]
    [PropertyOrder(3)]
    [BoxGroup("Dialogue")]
    public List<DialogueContext> DialogueContexts;

    [PropertyOrder(4)]
    [BoxGroup("Other")]
    public TMP_Text UILabel;

    public SENode ReferenceNode { get; set; }

    public void Update()
    {
        UILabel.text = ReferenceNode.Body.Label;
        gameObject.name = "Card: \"" + ReferenceNode.Body.Label + "\"";
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}