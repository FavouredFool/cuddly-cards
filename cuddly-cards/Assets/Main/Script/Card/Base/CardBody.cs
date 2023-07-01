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

    Vector3 _originalPosition;
    Quaternion _originalRotation;

    bool _isHovered = false;

    private void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        // Doing this every frame for every card really isnt cool

        // Reference to cardnode to check if the card is toplevel (this stuff is not necessary if the card isnt)

        SetHoveredHeight();

        _originalRotation = transform.rotation;
    }

    public void SetHoveredHeight()
    {
        if (!_isHovered)
        {
            transform.position = _originalPosition;
        }
        else
        {
            transform.position = _originalPosition + 0.33f * CardInfo.CARDRATIO * CardInfo.CARDWIDTH * Vector3.forward;
        }
    }

    void LateUpdate()
    {
        if (!_isHovered)
        {
            _originalPosition = transform.position;
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

    public Vector3 GetOriginalPosition()
    {
        return _originalPosition;
    }

    public Quaternion GetOriginalRotation()
    {
        return _originalRotation;
    }
}