using UnityEngine;

public class ModelObject : MonoBehaviour
{
    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public MeshRenderer GetMeshRenderer()
    {
        return _meshRenderer;
    }

    public void SetMesh(Mesh mesh)
    {
        _meshFilter.mesh = mesh;
    }
}
