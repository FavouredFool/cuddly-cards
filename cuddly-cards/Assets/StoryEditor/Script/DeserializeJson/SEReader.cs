using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SEReader
{
    int _count;
    TextAsset _textBlueprint;

    public SEReader(TextAsset text)
    {
        _textBlueprint = text;
    }

    public SENode ReadCards()
    {
        SEObject serializedObject = JsonConvert.DeserializeObject<SEObject>(_textBlueprint.text);
        SEObjectElement activeElement = serializedObject.elements[0];

        SENode rootNode = new(new(0, activeElement.Label, activeElement.Description, activeElement.Type));
        rootNode.Parent = null;

        _count = 1;
        int recursionDepth = 1;

        List<SEObjectElement> elementList = serializedObject.elements;
        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            rootNode.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return rootNode;
    }

    public SENode InitNodes(List<SEObjectElement> elementList, int recursionDepth)
    {
        SEObjectElement activeElement = elementList[_count];

        SEContext context = new(_count, activeElement.Label, activeElement.Description, activeElement.Type);

        if (activeElement.DesiredKey != null) context.DesiredKey = activeElement.DesiredKey;
        if (activeElement.TalkID != 0) context.TalkID = activeElement.TalkID;
        if (activeElement.Dialogue != null) context.DialogueContexts = activeElement.Dialogue;

        SENode node = new(context);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return node;
    }
}