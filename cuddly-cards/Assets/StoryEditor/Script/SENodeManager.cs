using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System;

public class SENodeManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] TextAsset _textBlueprint;

    [Header("Transforms")]
    [SerializeField] Transform _parentPoint;

    [SerializeField] Transform _childStartPoint;

    [Header("UI")]
    [SerializeField] TMP_Text _depthHigher;
    [SerializeField] TMP_Text _depthLower;

    public SENode RootNode { get; private set; }
    public SENode BaseNode { get; private set; }
    public SESaveLoadManager ReadWriter { get; private set; }
    public SENodeBuilder Builder { get; private set; }

    public Transform ParentPoint => _parentPoint;
    public Transform ChildStartPoint => _childStartPoint;

    public void Awake()
    {
        Builder = GetComponent<SENodeBuilder>();

        ReadWriter = new SESaveLoadManager();
    }

    public void Start()
    {
        SENode loadedNode = ReadWriter.OdinLoad();

        if (loadedNode == null) loadedNode = new(new(0, "Cover", "CoverDescription", CardInfo.CardType.COVER));

        RootNode = BaseNode = loadedNode;

        Builder.InitializeNodeTree(RootNode);

        SetBaseNode(RootNode);
    }

    public void Update()
    {
        SetNodeColor(BaseNode);
        foreach (SENode node in BaseNode.Children)
        {
            SetNodeColor(node);
        }
    }

    public void SaveToJSON()
    {
        ReadWriter.OdinSave(RootNode);
    }

    public void SetNodeColor(SENode node)
    {
        node.Body.SetColor(Builder.GetScriptableTypeFromCardType(node.SEObjectElement.CardType).GetCardColor());
    }

    public void SetBaseNode(SENode newBaseNode)
    {
        BaseNode = newBaseNode;

        OnlyEnableActiveNodes(newBaseNode);

        MoveNodeToParentPosition(newBaseNode);

        for (int i = 0; i < newBaseNode.Children.Count; i++)
        {
            MoveNodeToChildPosition(newBaseNode.Children[i], i);
        }

        _depthHigher.text = "Depth: " + BaseNode.SEObjectElement.Depth;
        _depthLower.text = "Depth: " + (BaseNode.SEObjectElement.Depth + 1);
    }

    public void MoveNodeToParentPosition(SENode node)
    {
        node.Body.transform.position = ParentPoint.position;
    }

    public void MoveNodeToChildPosition(SENode node, int index)
    {
        node.Body.transform.position = ChildStartPoint.position + Vector3.right * index * 3f;
    }

    public void AddChildNode()
    {
        // AUFGERUFEN ÜBER BUTTON

        if (BaseNode.Children.Count >= 4 && BaseNode.SEObjectElement.CardType != CardInfo.CardType.DWRAPPER && BaseNode.SEObjectElement.CardType != CardInfo.CardType.DIALOGUE)
        {
            Debug.LogWarning("This layer is full");
            return;
        }

        SENode node = new(new("defaultLabel", "defaultText", CardInfo.CardType.THING));

        node.Body = Builder.BuildCardBody(node);

        BaseNode.AddChild(node);

        //refresh
        SetBaseNode(BaseNode);
    }

    public void DeleteChild(int index)
    {
        if (BaseNode.Children.Count <= index)
        {
            return;
        }

        SENode node = BaseNode.Children[index];

        if (node.Children.Count != 0)
        {
            Debug.Log("not empty");
            return;
        }

        BaseNode.Children.Remove(node);
        Destroy(node.Body.gameObject);

        SetBaseNode(BaseNode);
    }

    public void SwapChildren(int index)
    {
        if (BaseNode.Children.Count <= index + 1)
        {
            return;
        }

        SENode leftNode = BaseNode.Children[index];

        BaseNode.Children.RemoveAt(index);
        BaseNode.Children.Insert(index + 1, leftNode);

        SetBaseNode(BaseNode);
    } 

    public void OnlyEnableActiveNodes(SENode referenceNode)
    {
        SetEnabledNodeAndChildren(false, RootNode);

        referenceNode.Body.gameObject.SetActive(true);
        foreach (SENode child in referenceNode.Children)
        {
            child.Body.gameObject.SetActive(true);
        }
    }

    public void SetEnabledNodeAndChildren(bool enabled, SENode referenceNode)
    {
        referenceNode.TraverseChildren(
            delegate (SENode SENode)
            {
                SENode.Body.gameObject.SetActive(enabled);
                return true;
            }
        );
    }
}
