using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SENodeManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] TextAsset _textBlueprint;

    [Header("Transforms")]
    [SerializeField] Transform _parentPoint;

    [SerializeField] List<Transform> _childPoints;

    [Header("UI")]
    [SerializeField] TMP_Text _depthHigher;
    [SerializeField] TMP_Text _depthLower;

    public SENode RootNode { get; private set; }
    public SENode BaseNode { get; private set; }
    public SEReader Reader { get; private set; } = null;
    public SESaveManager Saver { get; private set; }
    public SENodeBuilder Builder { get; private set; }

    public Transform ParentPoint => _parentPoint;
    public List<Transform> ChildPoints => _childPoints;

    public void Awake()
    {
        Builder = GetComponent<SENodeBuilder>();

        if (_textBlueprint != null)
        {
            Reader = new SEReader(_textBlueprint);
        }

        Saver = new SESaveManager();
    }

    public void Start()
    {
        if (Reader != null)
        {
            RootNode = BaseNode = Reader.ReadCards();
            
        }
        else
        {
            RootNode = BaseNode = new(new("Cover", "CoverDescription", CardInfo.CardType.COVER));
        }

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
        Saver.SaveToJSON(RootNode);
    }

    public void SetNodeColor(SENode node)
    {
        node.Body.SetColor(Builder.GetScriptableTypeFromCardType(node.Body.BodyContext.CardType).GetCardColor());
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

        _depthHigher.text = "Depth: " + BaseNode.Depth;
        _depthLower.text = "Depth: " + (BaseNode.Depth + 1);
    }

    public void MoveNodeToParentPosition(SENode node)
    {
        node.Body.transform.position = ParentPoint.position;
    }

    public void MoveNodeToChildPosition(SENode node, int index)
    {
        node.Body.transform.position = ChildPoints[index].position;
    }

    public void AddChildNode()
    {
        // AUFGERUFEN ÜBER BUTTON

        if (BaseNode.Children.Count >= 4)
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
