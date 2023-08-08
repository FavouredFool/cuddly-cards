using UnityEngine;

public class SEBody : MonoBehaviour
{
    public SENode CardReferenceNode { get; set; }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

}