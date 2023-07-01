using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CardBody : MonoBehaviour
{
    [SerializeField]
    TMP_Text _textField;

    [SerializeField]
    MeshRenderer _meshRenderer;

    [SerializeField]
    Image _image;

    Vector3 _originalLocalPosition;

    bool _isHovered = false;

    private void Start()
    {
        _originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        SetHoveredHeight();
    }

    public void SetHoveredHeight()
    {
        if (!_isHovered)
        {
            transform.localPosition = _originalLocalPosition;
        }
        else
        {
            transform.localPosition = _originalLocalPosition + Vector3.forward * CardInfo.CARDWIDTH * CardInfo.CARDRATIO * 0.33f;
        }
    }

    void LateUpdate()
    {
        if (!_isHovered)
        {
            _originalLocalPosition = transform.localPosition;
        }

        _isHovered = false;
    }

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
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            CardInfo.CARDHEIGHT * height,
            transform.localPosition.z
        );
    }

    public void SetHeight(float heightFloat)
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            CardInfo.CARDHEIGHT * heightFloat,
            transform.localPosition.z
        );
    }

    public void NodeIsHovered(float movementAmount)
    {
         _isHovered = true;
    }
}