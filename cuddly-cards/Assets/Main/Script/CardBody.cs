using UnityEngine;
using TMPro;

public class CardBody : MonoBehaviour
{
    [SerializeField]
    TMP_Text _textField;


    public void SetLabel(string labelText)
    {
        _textField.text = labelText;
    }

    public void SetPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
    }
}