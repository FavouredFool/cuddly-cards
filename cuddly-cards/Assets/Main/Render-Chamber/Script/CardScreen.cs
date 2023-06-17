using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class CardScreen : MonoBehaviour
{
    public Texture testTexture;

    MeshRenderer screen;
    RenderTexture viewTexture;
    Camera renderChamberCam;
    

    void Start()
    {
        //screen = GetComponent<MeshRenderer>();
        //renderChamberCam = GameObject.FindGameObjectWithTag("RenderCam").GetComponent<Camera>();
        //renderChamberCam.enabled = false;

        //CreateViewTexture();
        //RenderPipelineManager.beginContextRendering += OnBeginRendering;
    }



    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }

            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            renderChamberCam.targetTexture = viewTexture;

            screen.material.SetTexture("_MainTex", viewTexture);
        }
    }


}