using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CardReader
{
    int _count;
    TextAsset _textBlueprint;

    public CardReader(TextAsset text)
    {
        _textBlueprint = text;
    }

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

        if (activeElement.DesiredKey != null) context.DesiredKey = activeElement.DesiredKey;

        CardNode node = new(context);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return node;
    }
}