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
                SENode.Body = BuildCardBody(SENode);
                return true;
            }
        );
    }
    
    public SEBody BuildCardBody(SENode node)
    {
        SEBody body = GameObject.Instantiate(_nodeBlueprint, Vector3.zero, Quaternion.identity, _nodeFolder).GetComponent<SEBody>();
        body.gameObject.name = "Card: \"" + node.Context.Label + "\"";
        body.ReferenceNode = node;

        // Fill Context

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
}