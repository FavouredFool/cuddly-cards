using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class CardManager : MonoBehaviour
{

    [SerializeField]
    TextAsset _textBlueprint;
    public enum CardType { PLACE }

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;

    CardNode _rootNode;

    List<CardNode> _topLevelNodes;

    JsonTextReader _jsonReader;

    int _count;

    public void Awake()
    {
        _topLevelNodes = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
    }

    public void Start()
    {
        ParsedObject parsedObject = JsonConvert.DeserializeObject<ParsedObject>(_textBlueprint.text);

        _count = 0;

        ParsedObjectElement activeElement = parsedObject.elements[0];
        CardContext context = new(activeElement.Label, activeElement.Description, activeElement.Type);

        _rootNode = new(context);
        _rootNode.Parent = null;

        _count += 1;
        int recursionDepth = 1;

        List<ParsedObjectElement> elementList = parsedObject.elements;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            _rootNode.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        _cardBuilder.BuildAllCards(_rootNode);

        SetLayout(_rootNode);
    }

    public CardNode InitNodes(List<ParsedObjectElement> elementList, int recursionDepth)
    {
        ParsedObjectElement activeElement = elementList[_count];
        CardContext context = new(activeElement.Label, activeElement.Description, activeElement.Type);
        CardNode node = new(context);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return node;
    }

    public void SetLayout(CardNode mainNode)
    {
        ClearTopLevelNodes();

        SetTopNodes(mainNode);

        UpdatePiles();

        _cardMover.MoveCardsForLayout(mainNode, _rootNode);

        _cardInput.UpdateColliders();
    }

    void SetTopNodes(CardNode mainNode)
    {
        _topLevelNodes.Add(_rootNode);

        if (mainNode != _rootNode)
        {
            _topLevelNodes.Add(mainNode);

            if (mainNode.Parent != _rootNode)
            {
                _topLevelNodes.Add(mainNode.Parent);
            }
        }

        foreach (CardNode childNode in mainNode.Children)
        {
            _topLevelNodes.Add(childNode);
        }
    }

    void ClearTopLevelNodes()
    {
        _topLevelNodes.Clear();
    }

    void RefreshTopLevelForAllNodes()
    {
        _rootNode.TraverseContext(
            delegate (CardNode node)
            {
                node.IsTopLevel = _topLevelNodes.Contains(node);
                return true;
            });
    }

    void UpdatePiles()
    {
        RefreshTopLevelForAllNodes();

        _cardMover.ParentCards(_rootNode, _topLevelNodes);

        _cardMover.ResetPositionAndRotation(_rootNode);

        foreach (CardNode node in _topLevelNodes)
        {
            _cardMover.PileFromParenting(node);
        }
    }

    public List<CardNode> GetTopLevelNodes()
    {
        return _topLevelNodes;
    }

    public CardNode GetRootNode()
    {
        return _rootNode;
    }

}