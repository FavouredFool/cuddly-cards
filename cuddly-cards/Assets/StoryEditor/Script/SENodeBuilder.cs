using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

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
                SENode.Body = BuildCardBody(SENode, rootNode);
                return true;
            }
        );

        rootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                AddCrossReferences(SENode, rootNode);
                return true;
            }
        );
    }

    public void AddCrossReferences(SENode node, SENode rootNode)
    {
        node.Body.DesiredKey = (node.Context.DesiredKeyID != -1) ? GetNodeFromID(rootNode, node.Context.DesiredKeyID).Body : null;
        node.Body.DesiredTalk = (node.Context.DesiredTalkID != -1) ? GetNodeFromID(rootNode, node.Context.DesiredTalkID).Body : null;
    }
    
    public SEBody BuildCardBody(SENode node, SENode rootNode)
    {
        SEBody body = GameObject.Instantiate(_nodeBlueprint, Vector3.zero, Quaternion.identity, _nodeFolder).GetComponent<SEBody>();
        body.gameObject.name = "Card: \"" + node.Context.Label + "\"";
        body.ReferenceNode = node;

        // Fill Context
        body.CardType = node.Context.CardType;
        body.Label = node.Context.Label;
        body.Description = node.Context.Description;

        

        body.DialogueContexts = node.Context.DialogueContext;

        // Position
        if (node.Parent == null)
        {
            body.transform.position = _manager.ParentPoint.position;
        }

        return body;
    }

    public CardScriptableType GetScriptableTypeFromCardType(CardType type)
    {
        return _types.FirstOrDefault(e => e.GetCardType().Equals(type));
    }

    public SENode GetNodeFromID(SENode rootNode, int id)
    {
        SENode foundNode = null;

        rootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                if (SENode.Context.CardID == id)
                {
                    foundNode = SENode;
                }
                return foundNode == null;
            }
        );

        return foundNode;
    }
}