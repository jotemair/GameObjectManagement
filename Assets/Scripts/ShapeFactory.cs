using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    private List<Shape> _shapePrefabs = new List<Shape>();

    [SerializeField]
    private List<Material> _materials = new List<Material>();

    public Shape Get(int shapeID = 0, int materialID = 0)
    {
        Shape shapeInstance = Instantiate(_shapePrefabs[shapeID]);
        shapeInstance.ShapeID = shapeID;
        shapeInstance.SetMaterial(_materials[materialID], materialID);
        return shapeInstance;
    }

    public Shape GetRandom()
    {
        return Get(Random.Range(0, _shapePrefabs.Count), Random.Range(0, _materials.Count));
    }
}
