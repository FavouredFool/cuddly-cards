using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField]
    List<Mesh> _meshes;

    public Mesh GetMeshFromModelName(string meshName)
    {
        switch (meshName)
        {
            case "BaseObject":
                return _meshes[0];
            case "BasePerson":
                return _meshes[1];
            case "BasePlace":
                return _meshes[2];
        }

        return _meshes[3];
    }
}
