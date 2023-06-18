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

    public List<MeshRenderer> _meshRenderers;
    public Material _baseMat;

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

        RemoveFromCullingMask("Object1");
        RemoveFromCullingMask("Object2");

        AddToCullingMask("Object2");

        _renderCamera.targetTexture = _viewTextures[0];
        UniversalRenderPipeline.RenderSingleCamera(context, _renderCamera);

        RemoveFromCullingMask("Object2");
        AddToCullingMask("Object1");

        _renderCamera.targetTexture = _viewTextures[1];
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
