using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using AmazingAssets.AdvancedDissolve;

public class CardBody : MonoBehaviour
{
    [SerializeField] TMP_Text _textField;

    [SerializeField] MeshRenderer _meshRenderer;

    [SerializeField] Image _image;

    [SerializeField] Transform _cardContents;

    [SerializeField] float _disintegrateTime;


    public CardNode CardReferenceNode { get; set; }
    public Transform CardContents => _cardContents;

    Tween _hoverTween = null;

    public bool IsHovered { get; set; } = false;

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

    public void StartHoverTween()
    {
        IsHovered = true;

        if (_hoverTween.IsActive()) _hoverTween.Kill();

        float hoverPosition = CardInfo.CARDRATIO * CardInfo.CARDWIDTH * 0.33f;

        _hoverTween = _cardContents.DOLocalMoveZ(hoverPosition, CardInfo.HOVERSPEED).SetSpeedBased();
    }

    public void EndHoverTween()
    {
        IsHovered = false;

        if (_hoverTween.IsActive()) _hoverTween.Kill();

        _hoverTween = _cardContents.DOLocalMoveZ(0f, CardInfo.HOVERSPEED).SetSpeedBased();
    }

    public void ResetHover(CardNode hoveredNode)
    {
        if (CardReferenceNode == hoveredNode) return;

        if (!IsHovered) return;

        EndHoverTween();
    }

    public void DeactivateUI()
    {
        _image.enabled = false;
        _textField.enabled = false;
    }

    public async Task DisintegrateCard()
    {
        DeactivateUI();

        foreach (Material material in _meshRenderer.materials)
        {
            AdvancedDissolveKeywords.SetKeyword(material, AdvancedDissolveKeywords.State.Enabled, true);
        }

        float startTime = Time.time;
        float maxTime = startTime + _disintegrateTime;


        while (Time.time < maxTime)
        {
            float t = (Time.time - startTime) / _disintegrateTime;

            Debug.Log(t);

            foreach (Material material in _meshRenderer.materials)
            {
                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, t);
            }

            await Task.Yield();
        }
    }
}