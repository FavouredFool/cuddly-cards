using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;
using System;
using UnityEngine.Rendering.Universal;

public class RenderManager : MonoBehaviour
{
    [SerializeField]
    Camera _renderCamera;

    [SerializeField]
    Camera _mainCamera;

    [SerializeField]
    Material _baseMat;

    [SerializeField]
    CopyManager _copyManager;

    [SerializeField]
    CopyObject _cardCopyBlueprint;

    [SerializeField]
    Transform _cardModelFolder;

    List<RenderTexture> _viewTextures;

    List<CopyObject> _copyObjectList;

    List<CardNode> _topLevelNodes;

#pragma warning disable 0618
    [Obsolete]
#pragma warning restore 0618
    void Start()
    {
        _viewTextures = new();
        _copyObjectList = new();
        _topLevelNodes = new();

        _renderCamera.enabled = false;


        RecreateRenderTextures();

        RenderPipelineManager.beginContextRendering += OnBeginRendering;

        // Create 14 CardCopies -> A pool for all the renders
        for (int i = 0; i < 14; i++)
        {
            _copyObjectList.Add(Instantiate(_cardCopyBlueprint, _cardModelFolder));
        }

    }

    void RecreateRenderTextures()
    {

        // Old render texture needs to be killed manually

        for (int i = 0; i < _topLevelNodes.Count; i++)
        {
            if (i >= _viewTextures.Count)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _topLevelNodes[i].Body.GetMaskMeshRenderer().material = _baseMat;
                _topLevelNodes[i].Body.GetMaskMeshRenderer().material.SetTexture("_MainTex", newTexture);
                _viewTextures.Add(newTexture);
            }

            if (_viewTextures[i] == null || _viewTextures[i].width != Screen.width || _viewTextures[i].height != Screen.height)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _topLevelNodes[i].Body.GetMaskMeshRenderer().material.SetTexture("_MainTex", newTexture);

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
        if (!cameras.Contains(_mainCamera))
        {
            return;
        }

        if (_topLevelNodes.Count <= 0)
        {
            return;
        }

        _copyManager.UpdateObjects(_copyObjectList, _topLevelNodes);
        RecreateRenderTextures();

        for (int i = 0; i < _topLevelNodes.Count; i++)
        {
            _copyManager.ActivateCopyObject(_copyObjectList, i, _topLevelNodes.Count);
            _renderCamera.targetTexture = _viewTextures[i];
            UniversalRenderPipeline.RenderSingleCamera(context, _renderCamera);
        }
    }

    public void AddModel(CardNode cardNode)
    {
        // TODO _toplevelnodes wird doppelt gehalten very very bad
        _topLevelNodes.Add(cardNode);
    }

    public void ResetAllModels()
    {
        _topLevelNodes.Clear();

        foreach (RenderTexture texture in _viewTextures)
        {
            texture.Release();
        }
        _viewTextures.Clear();
    }
}
