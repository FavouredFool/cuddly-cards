using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEditorNodeManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] TextAsset _textBlueprint;

    public StoryEditorNode RootNode { get; private set; }
    public SEReader Reader { get; private set; } = null;

    public StoryEditorNodeBuilder Builder { get; private set; }

    public void Awake()
    {
        Builder = GetComponent<StoryEditorNodeBuilder>();

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

        // Turn off all nodes except those on the active layers.
        OnlyEnableActiveNodes(RootNode);


    }

    public void OnlyEnableActiveNodes(StoryEditorNode referenceNode)
    {
        SetEnabledNodeAndChildren(false, RootNode);

        referenceNode.Body.gameObject.SetActive(true);
        foreach (StoryEditorNode child in referenceNode.Children)
        {
            child.Body.gameObject.SetActive(true);
        }
    }

    public void SetEnabledNodeAndChildren(bool enabled, StoryEditorNode referenceNode)
    {
        referenceNode.TraverseChildren(
            delegate (StoryEditorNode SENode)
            {
                SENode.Body.gameObject.SetActive(false);
                return true;
            }
        );
    }
}
