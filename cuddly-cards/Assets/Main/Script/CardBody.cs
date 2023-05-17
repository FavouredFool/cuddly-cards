using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CardBody : MonoBehaviour
{
    [SerializeField]
    TMP_Text _textField;

    public void SetLabel(string labelText)
    {
        _textField.text = labelText;
    }

    public void SetHeight(int height)
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            CardInfo.CARDHEIGHT * height,
            transform.localPosition.z
        );
    }
}