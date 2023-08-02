using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using AmazingAssets.AdvancedDissolve;

public class CardBody : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;

    [SerializeField] TMP_Text _frontTextField;

    [SerializeField] Image _frontImage;

    [SerializeField] TMP_Text _backTextField;

    [SerializeField] Image _backImage;

    [SerializeField] Transform _cardContents;

    [SerializeField] float _disintegrateTime;

    public CardNode CardReferenceNode { get; set; }
    public Transform CardContents => _cardContents;
    public bool IsDisintegrating { get; set; } = false;

    Tween _hoverTween = null;

    Outline _outline;

    public bool IsHovered { get; set; } = false;

    public void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public void SetColor(Color color)
    {
        // just accessing the second element might be risky.
        _meshRenderer.materials[1].color = color;
    }
    public void SetFrontElements(string labelText, Sprite icon)
    {
        _frontTextField.text = labelText;
        _frontImage.sprite = icon;
    }

    public void SetBackElements(string labelText, Sprite icon)
    {
        _backTextField.text = labelText;
        _backImage.sprite = icon;
    }

    public void ClearBack()
    {
        _backImage.sprite = null;
        _backTextField.text = "";
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
        if (_hoverTween.IsActive()) _hoverTween.Kill();

        float hoverPosition = CardInfo.CARDRATIO * CardInfo.CARDWIDTH * 0.33f;

        _hoverTween = _cardContents.DOLocalMoveZ(hoverPosition, CardInfo.HOVERSPEED).SetSpeedBased();
    }

    public void EndHoverTween()
    {
        if (_hoverTween.IsActive()) _hoverTween.Kill();

        _hoverTween = _cardContents.DOLocalMoveZ(0f, CardInfo.HOVERSPEED).SetSpeedBased();
    }

    public void EndOutline()
    {
        _outline.enabled = false;

        foreach (CardNode child in CardReferenceNode.Children.Where(child => !child.IsTopLevel))
        {
            child.Body.EndOutline();
        }
    }

    public void StartOutline()
    {
        if (IsDisintegrating) return;

        _outline.enabled = true;

        foreach (CardNode child in CardReferenceNode.Children.Where(child => !child.IsTopLevel))
        {
            child.Body.StartOutline();
        }
    }

    public void DeactivateUI()
    {
        _frontImage.enabled = false;
        _frontTextField.enabled = false;
    }

    public async Task DisintegrateCard()
    {
        IsDisintegrating = true;

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

            foreach (Material material in _meshRenderer.materials)
            {
                AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, t);
            }

            await Task.Yield();
        }
    }
}