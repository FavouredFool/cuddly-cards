using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SESaveManager
{
    public void SaveToJSON(SENode rootNode)
    {
        SEObject objectToSerialize = new();

        objectToSerialize.Elements = FillObjectElementListFromRootNode(rootNode);

        string output = JsonConvert.SerializeObject(objectToSerialize);

        File.WriteAllText("Z:/Dokumente/Game Development/Unity/Repositories/cuddly-cards/cuddly-cards/Assets/Resources/GeneratedBlueprints/newBlueprint.json", output);

        Debug.Log("finished!");
        Debug.Log(output);
    }

    public List<SEObjectElement> FillObjectElementListFromRootNode(SENode rootNode)
    {
        List<SEObjectElement> objectElementList = new();

        rootNode.TraverseChildren(
            delegate (SENode SENode)
            {
                objectElementList.Add(GetObjectElementFromNode(SENode));
                return true;
            }
        );

        return objectElementList;
    }

    public SEObjectElement GetObjectElementFromNode(SENode node)
    {
        SEBodyContext bodyContext = node.Body.BodyContext;

        return new SEObjectElement(node.Depth, bodyContext.Label, bodyContext.Description, bodyContext.CardType);
    }
}
