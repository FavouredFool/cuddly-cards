using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class RenderManager : MonoBehaviour
{
    public List<MeshRenderer> _meshRenderers;
    public Material _baseMat;
    List<Camera> renderCameras;

    List<RenderTexture> viewTextures;

    CopyManager copyManager;

    void Start()
    {
        renderCameras = new();
        viewTextures = new();

        copyManager = GameObject.FindGameObjectWithTag("copyManager").GetComponent<CopyManager>();

        for (int i = 0; i < copyManager.copyList.Count; i++)
        {
            Camera cam = GameObject.FindGameObjectWithTag("RenderCam" + i).GetComponent<Camera>();
            RenderTexture tex = new(Screen.width, Screen.height, 0);

            cam.targetTexture = tex;
            //cam.enabled = false;

            _meshRenderers[i].material = _baseMat;
            _meshRenderers[i].material.SetTexture("_MainTex", tex);

            renderCameras.Add(cam);
            viewTextures.Add(tex);
        }

        //RenderPipelineManager.beginContextRendering += OnBeginRendering;

        

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

        UniversalRenderPipeline.RenderSingleCamera(context, renderCameras[0]);

        UniversalRenderPipeline.RenderSingleCamera(context, renderCameras[1]);
    }
}
