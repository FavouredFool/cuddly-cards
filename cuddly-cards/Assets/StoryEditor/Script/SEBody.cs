using UnityEngine;
using TMPro;

public class SEBody : MonoBehaviour
{
    [SerializeField] TMP_Text _label;

    public SENode ReferenceNode { get; set; }

    public SEBodyContext BodyContext { get; set; }

    public void Update()
    {
        _label.text = BodyContext.Label;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

}