using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryEditorNodeBuilder : MonoBehaviour
{
    [SerializeField] GameObject _nodeBlueprint;

    [Header("All Cardtypes")]
    [SerializeField] List<CardScriptableType> _types;

    [Header("Organization")]
    [SerializeField] Transform _nodeFolder;

    [Header("Transforms")]
    [SerializeField]
    Transform _parentPoint;

    [SerializeField]
    List<Transform> _childPoints;

    public void InitializeNodeTree(StoryEditorNode rootNode)
    {
        rootNode.TraverseChildren(
            delegate (StoryEditorNode SENode)
            {
                StoryEditorContext context = SENode.Context;
                SENode.Body = BuildCardBody(context, SENode);
                return true;
            }
        );
    }

    public StoryEditorBody BuildCardBody(StoryEditorContext context, StoryEditorNode nodeReference)
    {
        StoryEditorBody body = GameObject.Instantiate(_nodeBlueprint, Vector3.zero, Quaternion.identity, _nodeFolder).GetComponent<StoryEditorBody>();

        body.gameObject.name = "Card: \"" + context.Label + "\"";
        body.CardReferenceNode = nodeReference;

        CardScriptableType type = _types.FirstOrDefault(e => e.GetCardType().Equals(context.CardType));

        if (type == null)
        {
            Debug.LogError("Found no fitting type for card: " + context.CardType);
        }

        body.SetColor(type.GetCardColor());

        // Position
        if (nodeReference.Parent == null)
        {
            body.transform.position = _parentPoint.position;
        }
        else
        {
            int index = nodeReference.Parent.Children.IndexOf(nodeReference);
            body.transform.position = _childPoints[index].position;
        }

        return body;
    }

}