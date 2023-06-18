using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class RenderManager : MonoBehaviour
{
    public List<MeshRenderer> _meshRenderers;
    public Material _baseMat;
    Camera renderCamera;

    List<RenderTexture> viewTextures;

    CopyManager copyManager;

    void Start()
    {
        viewTextures = new();

        renderCamera = GameObject.FindGameObjectWithTag("RenderCam1").GetComponent<Camera>();
        copyManager = GameObject.FindGameObjectWithTag("copyManager").GetComponent<CopyManager>();

        for (int i = 0; i < copyManager.copyList.Count; i++)
        {
            RenderTexture tex = new(Screen.width, Screen.height, 0);

            renderCamera.enabled = false;

            _meshRenderers[i].material = _baseMat;
            _meshRenderers[i].material.SetTexture("_MainTex", tex);

            viewTextures.Add(tex);
        }

        RenderPipelineManager.beginContextRendering += OnBeginRendering;

        

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

        RemoveFromCullingMask("Object1");
        RemoveFromCullingMask("Object2");

        AddToCullingMask("Object2");

        renderCamera.targetTexture = viewTextures[0];
        UniversalRenderPipeline.RenderSingleCamera(context, renderCamera);

        RemoveFromCullingMask("Object2");
        AddToCullingMask("Object1");

        renderCamera.targetTexture = viewTextures[1];
        UniversalRenderPipeline.RenderSingleCamera(context, renderCamera);
    }

    void AddToCullingMask(string layerName)
    {
        renderCamera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
    }

    void RemoveFromCullingMask(string layerName)
    {
        renderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
    }
}
