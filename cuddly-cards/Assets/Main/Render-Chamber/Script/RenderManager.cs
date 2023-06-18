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
    List<MeshRenderer> _meshRenderers;

    [SerializeField]
    Material _baseMat;

    [SerializeField]
    CopyManager _copyManager;

    List<RenderTexture> _viewTextures;

#pragma warning disable 0618
    [Obsolete]
#pragma warning restore 0618
    void Start()
    {
        _viewTextures = new();

        _renderCamera.enabled = false;


        RecreateRenderTextures();

        RenderPipelineManager.beginContextRendering += OnBeginRendering;
    }

    void RecreateRenderTextures()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            if (i >= _viewTextures.Count)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _meshRenderers[i].material = _baseMat;
                _meshRenderers[i].material.SetTexture("_MainTex", newTexture);
                _viewTextures.Add(newTexture);
            }

            if (_viewTextures[i].width != Screen.width || _viewTextures[i].height != Screen.height)
            {
                RenderTexture newTexture = new(Screen.width, Screen.height, 0);
                _meshRenderers[i].material.SetTexture("_MainTex", newTexture);

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

        if (_meshRenderers.Count <= 0)
        {
            return;
        }
        
        RecreateRenderTextures();

        _copyManager.SetCopyActive(0, false);
        _copyManager.SetCopyActive(1, false);
        _copyManager.SetCopyActive(2, false);

        _copyManager.SetCopyActive(0, true);

        _renderCamera.targetTexture = _viewTextures[0];
        UniversalRenderPipeline.RenderSingleCamera(context, _renderCamera);

        _copyManager.SetCopyActive(0, false);
        _copyManager.SetCopyActive(1, true);

        _renderCamera.targetTexture = _viewTextures[1];
        UniversalRenderPipeline.RenderSingleCamera(context, _renderCamera);

        _copyManager.SetCopyActive(1, false);
        _copyManager.SetCopyActive(2, true);

        _renderCamera.targetTexture = _viewTextures[2];
        UniversalRenderPipeline.RenderSingleCamera(context, _renderCamera);
    }
}
