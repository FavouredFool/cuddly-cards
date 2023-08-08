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
            RootNode = new();
        }

        Builder.InitializeNodeTree(RootNode);





    }
}
