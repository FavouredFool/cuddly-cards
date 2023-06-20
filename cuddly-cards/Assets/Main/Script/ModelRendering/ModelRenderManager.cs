using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;
using System;
using UnityEngine.Rendering.Universal;

public class ModelRenderManager : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField]
    CardManager _cardManager;

    [Header("Cameras")]
    [SerializeField]
    ModelCameraMovement _renderCameraMovement;

    [SerializeField]
    CameraMovement _mainCameraMovement;

    [Header("CardCopy")]
    [SerializeField]
    ModelObject _cardModelBlueprint;

    [SerializeField]
    Material _baseMat;

    [SerializeField]
    Transform _cardModelFolder;

    List<RenderTexture> _viewTextures;

    List<ModelObject> _modelObjectList;

#pragma warning disable 0618
    [Obsolete]
#pragma warning restore 0618
    void Start()
    {
        _viewTextures = new();
        _modelObjectList = new();

        _renderCameraMovement.GetCamera().enabled = false;


        RecreateRenderTextures();

        RenderPipelineManager.beginContextRendering += OnBeginRendering;

        // Create 14 CardCopies -> A pool for all the renders
        for (int i = 0; i < 14; i++)
        {
            _modelObjectList.Add(Instantiate(_cardModelBlueprint, _cardModelFolder));
        }

    }

    void RecreateRenderTextures()
    {
        // Old render texture needs to be killed manually

        for (int i = 0; i < _cardManager.GetTopLevelNodes().Count; i++)
        {
            if (i >= _viewTextures.Count)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _cardManager.GetTopLevelNodes()[i].Body.GetMaskMeshRenderer().material = _baseMat;
                _cardManager.GetTopLevelNodes()[i].Body.GetMaskMeshRenderer().material.SetTexture("_MainTex", newTexture);
                _viewTextures.Add(newTexture);
            }

            if (_viewTextures[i] == null || _viewTextures[i].width != Screen.width || _viewTextures[i].height != Screen.height)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _cardManager.GetTopLevelNodes()[i].Body.GetMaskMeshRenderer().material.SetTexture("_MainTex", newTexture);

                _viewTextures.RemoveAt(i);
                _viewTextures.Insert(i, newTexture);
            }
        }
    }

#pragma warning disable 0618
    [Obsolete]
#pragma warning restore 0618
    void OnBeginRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (!Application.isPlaying)
        {
            return;
        }
        // make sure that only the relevant renders are allowed. Only if the mainCamera is part of the render, the render is relevant.
        if (!cameras.Contains(_mainCameraMovement.GetCamera()))
        {
            return;
        }

        if (_cardManager.GetTopLevelNodes().Count <= 0)
        {
            return;
        }

        UpdateObjects(_modelObjectList, _cardManager.GetTopLevelNodes());
        RecreateRenderTextures();

        for (int i = 0; i < _cardManager.GetTopLevelNodes().Count; i++)
        {
            ActivateObjectCopy(_modelObjectList, i, _cardManager.GetTopLevelNodes().Count);
            _renderCameraMovement.GetCamera().targetTexture = _viewTextures[i];
            UniversalRenderPipeline.RenderSingleCamera(context, _renderCameraMovement.GetCamera());
        }
    }

    void UpdateObjects(List<ModelObject> copyObjectList, List<CardNode> topLevelCards)
    {
        copyObjectList.ForEach(e => e.gameObject.SetActive(false));
        _renderCameraMovement.SetModelTransform(_mainCameraMovement.transform);

        for (int i = 0; i < topLevelCards.Count; i++)
        {
            copyObjectList[i].gameObject.SetActive(true);
            copyObjectList[i].GetComponent<ModelTransform>().SetModelTransform(topLevelCards[i].Body.transform);
        }
    }

    void ActivateObjectCopy(List<ModelObject> copyObjectList, int index, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            copyObjectList[i].CopyObjectRenderer.enabled = i == index;
        }
    }

    public void ResetAllModels()
    {
        foreach (RenderTexture texture in _viewTextures)
        {
            texture.Release();
        }
        _viewTextures.Clear();
    }
}
