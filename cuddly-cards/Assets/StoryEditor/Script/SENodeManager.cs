using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SENodeManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] TextAsset _textBlueprint;

    public SENode RootNode { get; private set; }
    public SEReader Reader { get; private set; } = null;

    public SENodeBuilder Builder { get; private set; }

    public void Awake()
    {
        Builder = GetComponent<SENodeBuilder>();

        if (_textBlueprint != null)
        {
            Reader = new SEReader(_textBlueprint);
        }
    }

    public void Start()
    {
        if (Reader != null)
        {
            RootNode = Reader.ReadCards();
            
        }
        else
        {
            RootNode = new(new("Cover", "CoverDescription", CardInfo.CardType.COVER));
        }

        Builder.InitializeNodeTree(RootNode);

        OnlyEnableActiveNodes(RootNode);
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
