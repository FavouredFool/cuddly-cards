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

    [SerializeField]
    ModelManager _modelManager;

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

    // The amount of models that you need is unfortunately variable because the amount of top-level cards is variable. When a stack sorts into another stack, the top-level gets increased dynamically so that only part of the deck gets moved upwards.
    int MODELAMOUNT = 32;

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

        // Create 15 CardCopies -> A pool for all the renders
        for (int i = 0; i < MODELAMOUNT; i++)
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
            CardNode cardNode = _cardManager.GetTopLevelNodes()[i];
            Color backgroundColor = cardNode.Context.GetBackgroundColor();
            _renderCameraMovement.GetCamera().backgroundColor = backgroundColor;
            UniversalRenderPipeline.RenderSingleCamera(context, _renderCameraMovement.GetCamera());
        }
    }

    void UpdateObjects(List<ModelObject> modelObjectList, List<CardNode> topLevelCards)
    {
        modelObjectList.ForEach(e => e.gameObject.SetActive(false));
        _renderCameraMovement.SetModelTransform(_mainCameraMovement.transform);

        for (int i = 0; i < topLevelCards.Count; i++)
        {
            modelObjectList[i].gameObject.SetActive(true);
            modelObjectList[i].GetComponent<ModelTransform>().SetModelTransform(topLevelCards[i].Body.transform);
        }
    }

    void ActivateObjectCopy(List<ModelObject> copyObjectList, int index, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            copyObjectList[i].GetMeshRenderer().enabled = i == index;
        }
    }

    public void SetModel(CardNode node)
    {
        int index = _cardManager.GetTopLevelNodes().IndexOf(node);

        _modelObjectList[index].SetMesh(_modelManager.GetMeshFromModelName(node.Context.GetModelName()));
    }

    public void ResetModels()
    {
        foreach (RenderTexture texture in _viewTextures)
        {
            texture.Release();
            
        }
        _viewTextures.Clear();

        // Reset models
        foreach (ModelObject model in _modelObjectList)
        {
            // new Mesh() ? 
            model.SetMesh(null);
        }
    }
}
