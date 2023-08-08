using UnityEngine;

public class SEBody : MonoBehaviour
{
    public SENode ReferenceNode { get; set; }

    public SEBodyContext BodyContext { get; set; }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

}