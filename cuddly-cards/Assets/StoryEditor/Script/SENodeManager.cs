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

        ValidateCurrentTree();

        SetNodeColor(BaseNode);
        foreach (SENode node in BaseNode.Children)
        {
            SetNodeColor(node);
        }
    }

    public void ValidateCurrentTree()
    {
        RootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                ValidateCurrentNode(SENode);
                return true;
            }
        );
    }

    public void ValidateCurrentNode(SENode node)
    {
        ValidateDialogue(node);
        ValidateCurrentNodeChildAmount(node);
        RestrictCardTypesForChildren(node);
        ValidateReferences(node);
    }

    public void ValidateDialogue(SENode node)
    {
        if (node.Body.CardType != CardInfo.CardType.DIALOGUE)
        {
            return;
        }

        if (node.Body.DialogueContexts == null || node.Body.DialogueContexts.Count == 0)
        {
            Debug.LogWarning(node.Body.name + " has no dialogue.");
        }

        // check that only the first X children are locks with X being the amount of locks in dialogue
        int count = 0;

        foreach (DialogueContext context in node.Body.DialogueContexts)
        {
            if (context.IsLockDialogue)
            {
                count += 1;
            }
        }

        bool flagged = false;

        foreach (SENode child in node.Children)
        {
            if (count > 0)
            {
                // should be lock
                if (child.Body.CardType != CardInfo.CardType.LOCK)
                {
                    flagged = true;
                }
            }
            else
            {
                // should not be lock
                if (child.Body.CardType == CardInfo.CardType.LOCK)
                {
                    flagged = true;
                }
            }
            count -= 1;
        }

        if (flagged)
        {
            Debug.LogWarning(node.Body.name + " is not lock-synchronized");
        }
        

    }

    public void ValidateReferences(SENode node)
    {
        switch (node.Body.CardType)
        {
            case CardInfo.CardType.LOCK:
                if (node.Body.DesiredKey == null || node.Body.DesiredKey.CardType != CardInfo.CardType.KEY)
                {
                    Debug.LogWarning(node.Body.name + " has no key.");
                }

                break;

            case CardInfo.CardType.DIALOGUE:

                if (node.Body.DesiredTalk == null && node.Parent.Body.CardType == CardInfo.CardType.DWRAPPER)
                {
                    Debug.LogWarning(node.Body.name + " has no talk-reference.");
                }

                if (node.Body.DesiredTalk != null && node.Body.DesiredTalk.CardType != CardInfo.CardType.TALK)
                {
                    Debug.LogWarning(node.Body.name + " talk has wrong type.");
                }

                break;

            default:
                break;
        }
    }

    public void RestrictCardTypesForChildren(SENode node)
    {
        switch (node.Body.CardType)
        {
            case CardInfo.CardType.TALK:
                foreach (SENode child in node.Children)
                {
                    if (child.Body.CardType != CardInfo.CardType.DIALOGUE)
                    {
                        Debug.LogWarning(node.Body.name + " has invalid children-cardtypes");
                    }
                }
                break;

            case CardInfo.CardType.DIALOGUE:

                foreach (SENode child in node.Children)
                {
                    if (child.Body.CardType == CardInfo.CardType.DWRAPPER || child.Body.CardType == CardInfo.CardType.KEY || child.Body.CardType == CardInfo.CardType.LOCK)
                    {
                        continue;
                    }

                    Debug.LogWarning(node.Body.name + " has invalid children-cardtypes");
                }
                break;

            case CardInfo.CardType.DWRAPPER:

                foreach (SENode child in node.Children)
                {
                    if (child.Body.CardType == CardInfo.CardType.DIALOGUE)
                    {
                        continue;
                    }

                    Debug.LogWarning(node.Body.name + " has invalid children-cardtypes");
                }
                break;

            default:
                break;
        }
    }

    public void ValidateCurrentNodeChildAmount(SENode node)
    {
        switch (node.Body.CardType)
        {
            case CardInfo.CardType.LOCK:
                if (node.Children.Count > 1)
                {
                    Debug.LogWarning(node.Body.name + " has too many children.");
                }
                break;
            case CardInfo.CardType.TALK:
            case CardInfo.CardType.DIALOGUE:
            case CardInfo.CardType.DWRAPPER:
                break;

            default:
                if (node.Children.Count > 4)
                {
                    Debug.LogWarning(node.Body.name + " has too many children.");
                }
                break;
        }
    }

    public void SaveToJSON()
    {
        ReadWriter.OdinSave(RootNode);
    }

    public void SetNodeColor(SENode node)
    {
        node.Body.SetColor(Builder.GetScriptableTypeFromCardType(node.Body.CardType).GetCardColor());
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

        _depthHigher.text = "Depth: " + BaseNode.Context.Depth;
        _depthLower.text = "Depth: " + (BaseNode.Context.Depth + 1);
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

        SENode node = new(new("defaultLabel", "defaultText", CardInfo.CardType.THING));

        node.Body = Builder.BuildCardBody(node, RootNode);

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
