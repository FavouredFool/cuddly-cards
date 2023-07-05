using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CardBody : MonoBehaviour
{
    [SerializeField] TMP_Text _textField;

    [SerializeField] MeshRenderer _meshRenderer;

    [SerializeField] Image _image;

    [SerializeField] Transform _cardContents;

    public Transform CardContents => _cardContents;

    public void SetLabel(string labelText)
    {
        _textField.text = labelText;
    }

    public void SetColor(Color color)
    {
        // just accessing the second element might be risky.
        _meshRenderer.materials[1].color = color;
    }

    public void SetIcon(Sprite icon)
    {
        _image.sprite = icon;
    }

    public void SetHeight(int height)
    {
        SetHeightFloat(height);
    }

    public void SetHeightFloat(float heightFloat)
    {
        var localPosition = transform.localPosition;

        localPosition = new Vector3(
            localPosition.x,
            CardInfo.CARDHEIGHT * heightFloat,
            localPosition.z
        );
        transform.localPosition = localPosition;
    }

    public void SetHoverPosition(bool isHovering)
    {
        float multiplier = isHovering ? 0.33f : 0;

        Vector3 position = _cardContents.localPosition;
        _cardContents.localPosition = new Vector3(position.x, position.y, CardInfo.CARDRATIO * CardInfo.CARDWIDTH * multiplier);
    }
}