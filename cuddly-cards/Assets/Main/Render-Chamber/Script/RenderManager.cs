using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class RenderManager : MonoBehaviour
{
    public List<MeshRenderer> _meshRenderers;
    Camera renderCamera;

    RenderTexture viewTexture;

    CopyManager copyManager;

    void Start()
    {
        viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderCamera = GameObject.FindGameObjectWithTag("RenderCam").GetComponent<Camera>();
        renderCamera.targetTexture = viewTexture;
        renderCamera.enabled = false;

        _meshRenderers[0].material.SetTexture("_MainTex", viewTexture);

        RenderPipelineManager.beginContextRendering += OnBeginRendering;

        copyManager = GameObject.FindGameObjectWithTag("copyManager").GetComponent<CopyManager>();

    }

    void SetViewTexture(MeshRenderer screen)
    {
        
    }

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

        SetViewTexture(_meshRenderers[0]);
        copyManager.SetEnabledObjects(0, true);
        copyManager.SetEnabledObjects(1, false);
        UniversalRenderPipeline.RenderSingleCamera(context, renderCamera);

        SetViewTexture(_meshRenderers[1]);
        copyManager.SetEnabledObjects(0, false);
        copyManager.SetEnabledObjects(1, true);
        UniversalRenderPipeline.RenderSingleCamera(context, renderCamera);
    }
}
