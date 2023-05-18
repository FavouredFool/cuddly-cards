using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CardReader : MonoBehaviour
{
    [SerializeField]
    TextAsset _textBlueprint;

    int _count;

    public CardNode ReadCards()
    {
        DeserializedObject serializedObject = JsonConvert.DeserializeObject<DeserializedObject>(_textBlueprint.text);
        DeserializedObjectElement activeElement = serializedObject.elements[0];

        CardNode rootNode = new(new(activeElement.Label, activeElement.Description, activeElement.Type));
        rootNode.Parent = null;

        _count = 1;
        int recursionDepth = 1;

        List<DeserializedObjectElement> elementList = serializedObject.elements;
        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            rootNode.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return rootNode;
    }

    public CardNode InitNodes(List<DeserializedObjectElement> elementList, int recursionDepth)
    {
        DeserializedObjectElement activeElement = elementList[_count];
        CardContext context = new(activeElement.Label, activeElement.Description, activeElement.Type);
        CardNode node = new(context);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return node;
    }
}