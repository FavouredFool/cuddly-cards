using UnityEngine;
using TMPro;
using System;
using Sirenix.OdinInspector;
using static CardInfo;

public class SEBody : MonoBehaviour
{
    [BoxGroup("Basics")]
    [ShowInInspector]
    public CardType CardType
    {
        get { return this.ReferenceNode.SEObjectElement.CardType; }
        set { this.ReferenceNode.SEObjectElement.CardType = value; }
    }

    [BoxGroup("Basics")]
    [MultiLineProperty(2)]
    [ShowInInspector]
    public string Label
    {
        get { return this.ReferenceNode.SEObjectElement.Label; }
        set { this.ReferenceNode.SEObjectElement.Label = value; }
    }
    [BoxGroup("Basics")]
    [MultiLineProperty(5)]
    [ShowInInspector]
    public string Description
    {
        get { return this.ReferenceNode.SEObjectElement.Description; }
        set { this.ReferenceNode.SEObjectElement.Description = value; }
    }


    [SerializeField] TMP_Text _label;

    public SENode ReferenceNode { get; set; }

    public void Update()
    {
        _label.text = ReferenceNode.SEObjectElement.Label;
        gameObject.name = "Card: \"" + ReferenceNode.SEObjectElement.Label + "\"";
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}