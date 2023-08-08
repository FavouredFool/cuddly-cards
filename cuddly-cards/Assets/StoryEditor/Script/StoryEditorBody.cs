using UnityEngine;

public class StoryEditorBody : MonoBehaviour
{
    public StoryEditorNode CardReferenceNode { get; set; }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

}