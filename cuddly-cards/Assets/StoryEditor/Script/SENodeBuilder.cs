using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SENodeBuilder : MonoBehaviour
{
    [SerializeField] GameObject _nodeBlueprint;

    [Header("All Cardtypes")]
    [SerializeField] List<CardScriptableType> _types;

    [Header("Organization")]
    [SerializeField] Transform _nodeFolder;

    SENodeManager _manager;

    public void Awake()
    {
        _manager = GetComponent<SENodeManager>();
    }

    public void InitializeNodeTree(SENode rootNode)
    {
        rootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                SEContext context = SENode.Context;
                SENode.Body = BuildCardBody(context, SENode);
                return true;
            }
        );
    }
    
    public SEBody BuildCardBody(SEContext context, SENode nodeReference)
    {
        SEBody body = GameObject.Instantiate(_nodeBlueprint, Vector3.zero, Quaternion.identity, _nodeFolder).GetComponent<SEBody>();
        body.gameObject.name = "Card: \"" + context.Label + "\"";
        body.ReferenceNode = nodeReference;

        body.BodyContext = body.GetComponent<SEBodyContext>();
        body.BodyContext.InitializeBodyContext(context.ID, context.Label, context.Description, context.CardType, context.DesiredKey, context.TalkID, context.DialogueContexts);

        CardScriptableType type = _types.FirstOrDefault(e => e.GetCardType().Equals(context.CardType));

        if (type == null)
        {
            Debug.LogError("Found no fitting type for card: " + context.CardType);
        }

        body.SetColor(type.GetCardColor());

        // Position
        if (nodeReference.Parent == null)
        {
            body.transform.position = _manager.ParentPoint.position;
        }
        else
        {
            int index = nodeReference.Parent.Children.IndexOf(nodeReference);
            body.transform.position = _manager.ChildPoints[index].position;
        }

        return body;
    }
}