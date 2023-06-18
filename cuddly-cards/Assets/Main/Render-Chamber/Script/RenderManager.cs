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
    List<MeshRenderer> _meshRenderers;

    [SerializeField]
    Material _baseMat;

    [SerializeField]
    CopyManager _copyManager;

    List<RenderTexture> _viewTextures;

    void Start()
    {
        _viewTextures = new();

        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            RenderTexture tex = new(Screen.width, Screen.height, 0);

            _renderCamera.enabled = false;

            _meshRenderers[i].material = _baseMat;
            _meshRenderers[i].material.SetTexture("_MainTex", tex);

            _viewTextures.Add(tex);
        }

        RenderPipelineManager.beginContextRendering += OnBeginRendering;
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

        if (_meshRenderers.Count <= 0)
        {
            return;
        }

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

    void AddToCullingMask(string layerName)
    {
        _renderCamera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
    }

    void RemoveFromCullingMask(string layerName)
    {
        _renderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
    }
}
