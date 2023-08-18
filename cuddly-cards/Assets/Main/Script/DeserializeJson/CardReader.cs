using Sirenix.Serialization;
using System.Collections.Generic;
using System.IO;
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
        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes("Z:/Dokumente/Game Development/Unity/Repositories/cuddly-cards/cuddly-cards/Assets/Resources/GeneratedBlueprints/newBlueprint.json");
        }
        catch
        {
            return null;
        }

        List<SEContext> objectElementList = SerializationUtility.DeserializeValue<List<SEContext>>(bytes, DataFormat.JSON);

        SEContext activeElement = objectElementList[0];

        CardNode rootNode = new(new(activeElement.CardID, activeElement.CardType, activeElement.Label, activeElement.Description, activeElement.DesiredKeyID, activeElement.DesiredTalkID, activeElement.DialogueContext));
        rootNode.Parent = null;

        _count = 1;
        int recursionDepth = 1;

        while (_count < objectElementList.Count && objectElementList[_count].Depth == recursionDepth)
        {
            rootNode.AddChild(InitNodes(objectElementList, recursionDepth + 1));
        }

        return rootNode;
    }

    public CardNode InitNodes(List<SEContext> elementList, int recursionDepth)
    {
        SEContext activeElement = elementList[_count];

        CardContext context = new(activeElement.CardID, activeElement.CardType, activeElement.Label, activeElement.Description, activeElement.DesiredKeyID, activeElement.DesiredTalkID, activeElement.DialogueContext);

        CardNode node = new(context);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1));
        }

        return node;
    }
}