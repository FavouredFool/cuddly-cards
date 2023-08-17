using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.Serialization;

public class SESaveLoadManager
{
    int _count = 0;

    public void OdinSave(SENode rootNode)
    {
        List<SEContext> objectElementList = FillObjectElementListFromRootNode(rootNode);

        byte[] bytes = SerializationUtility.SerializeValue(objectElementList, DataFormat.JSON);

        File.WriteAllBytes("Z:/Dokumente/Game Development/Unity/Repositories/cuddly-cards/cuddly-cards/Assets/Resources/GeneratedBlueprints/newBlueprint.json", bytes);
    }

    public SENode OdinLoad()
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

        // Distribute
        SEContext activeElement = objectElementList[0];

        SENode rootNode = new(new(0, activeElement.Label, activeElement.Description, activeElement.CardType));
        rootNode.Parent = null;

        _count = 1;
        int recursionDepth = 1;

        while (_count < objectElementList.Count && objectElementList[_count].Depth == recursionDepth)
        {
            rootNode.AddChild(InitNodes(objectElementList, recursionDepth + 1), recursionDepth);
        }

        return rootNode;
    }

    public SENode InitNodes(List<SEContext> elementList, int recursionDepth)
    {
        SEContext activeElement = elementList[_count];

        SEContext objectElement = new(_count, activeElement.Label, activeElement.Description, activeElement.CardType);

        SENode node = new(objectElement);

        _count += 1;

        while (_count < elementList.Count && elementList[_count].Depth == recursionDepth)
        {
            node.AddChild(InitNodes(elementList, recursionDepth + 1), recursionDepth);
        }

        return node;
    }

    public List<SEContext> FillObjectElementListFromRootNode(SENode rootNode)
    {
        List<SEContext> objectElementList = new();

        rootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                objectElementList.Add(SENode.SEObjectElement);
                return true;
            }
        );

        return objectElementList;
    }
}
