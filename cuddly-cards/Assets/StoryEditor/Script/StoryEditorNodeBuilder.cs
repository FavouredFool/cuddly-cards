using UnityEngine;
using System.Collections.Generic;

public class StoryEditorNodeBuilder : MonoBehaviour
{
    [SerializeField] GameObject _nodeBlueprint;

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
                //CardContext context = cardNode.Context;
                SENode.Body = BuildCardBody(SENode);
                return true;
            }
        );
    }

    public StoryEditorBody BuildCardBody(StoryEditorNode nodeReference)
    {
        StoryEditorBody body = GameObject.Instantiate(_nodeBlueprint, Vector3.zero, Quaternion.identity, _nodeFolder).GetComponent<StoryEditorBody>();

        //body.gameObject.name = "Card: \"" + cardContext.Label + "\"";
        body.CardReferenceNode = nodeReference;

        //CardScriptableType type = _types.FirstOrDefault(e => e.GetCardType().Equals(cardContext.CardType));

        /*
        if (type == null)
        {
            Debug.LogError("Found no fitting type for card: " + cardContext.CardType);
        }
        */

        // Position
        
        if (nodeReference.Parent == null)
        {
            body.transform.position = _parentPoint.position;
        }
        else
        {
            body.transform.position = _childPoints[nodeReference.Parent.Children.IndexOf(nodeReference)].position;
        }

        return body;
    }

}