using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    // List of possible shapes
    [SerializeField]
    private List<Shape> _shapePrefabs = new List<Shape>();

    // List of possible materials
    [SerializeField]
    private List<Material> _materials = new List<Material>();

    // Should we recycle removed objects or not
    [SerializeField]
    private bool _recycle = false;

    // The pool of shape instances for each shape type
    private List<List<Shape>> _pools = null;

    // The scene where the pooled objects are stored
    private Scene _poolScene = default;

    // Instantiate a shape through the factory
    public Shape Get(int shapeID = 0, int materialID = 0)
    {
        Shape shapeInstance = null;

        if (_recycle)
        {
            if (null == _pools)
            {
                CreatePools();
            }

            List<Shape> pool = _pools[shapeID];

            if (pool.Count > 0)
            {
                shapeInstance = pool[pool.Count - 1];
                shapeInstance.gameObject.SetActive(true);
                pool.RemoveAt(pool.Count - 1);
            }
            else
            {
                shapeInstance = Instantiate(_shapePrefabs[shapeID]);
                shapeInstance.ShapeID = shapeID;
                SceneManager.MoveGameObjectToScene(shapeInstance.gameObject, _poolScene);
            }
        }
        else
        {
            shapeInstance = Instantiate(_shapePrefabs[shapeID]);
            shapeInstance.ShapeID = shapeID;
        }

        shapeInstance.SetMaterial(_materials[materialID], materialID);
        shapeInstance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        return shapeInstance;
    }

    // Reclaim shape instances that are no longer needed
    public void Reclaim(Shape shapeToRecycle)
    {
        if (_recycle)
        {
            if (_pools == null)
            {
                CreatePools();
            }
            _pools[shapeToRecycle.ShapeID].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shapeToRecycle.gameObject);
        }
    }

    // Get a random shape
    public Shape GetRandom()
    {
        return Get(Random.Range(0, _shapePrefabs.Count), Random.Range(0, _materials.Count));
    }

    // Create object pools for reclaimed shape instances
    private void CreatePools()
    {
        _pools = new List<List<Shape>>();

        foreach (var shape in _shapePrefabs)
        {
            _pools.Add(new List<Shape>());
        }


        if (Application.isEditor)
        {
            _poolScene = SceneManager.GetSceneByName(name);
            if (_poolScene.isLoaded)
            {
                GameObject[] rootObjects = _poolScene.GetRootGameObjects();
                foreach (var shapeObj in rootObjects)
                {
                    Shape pooledShape = shapeObj.GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        _pools[pooledShape.ShapeID].Add(pooledShape);
                    }
                }

                return;
            }
        }

        _poolScene = SceneManager.CreateScene(name);
    }
}
