using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    const int saveVersion = 2;

    [SerializeField]
    public ShapeFactory _shapeFactory = null;

    [SerializeField]
    private KeyCode _createKey = KeyCode.C;

    [SerializeField]
    private KeyCode _newGameKey = KeyCode.N;

    [SerializeField]
    private KeyCode _saveGameKey = KeyCode.S;

    [SerializeField]
    private KeyCode _loadGameKey = KeyCode.L;

    private List<Shape> _shapes = new List<Shape>();

    [SerializeField]
    private PersistentStorage _storage = null;

    private void Update()
    {
        if (Input.GetKeyDown(_createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(_newGameKey))
        {
            StartNewGame();
        }
        else if (Input.GetKeyDown(_saveGameKey))
        {
            SaveGame();
        }
        else if (Input.GetKeyDown(_loadGameKey))
        {
            LoadGame();
        }
    }

    private void CreateShape()
    {
        Shape shapeInstance = _shapeFactory.GetRandom();
        Transform trans = shapeInstance.transform;
        trans.position = Random.insideUnitSphere * 5f;
        trans.rotation = Random.rotation;
        trans.localScale = Vector3.one * Random.Range(0.1f, 1f);

        _shapes.Add(shapeInstance);
    }

    private void StartNewGame()
    {
        foreach (var persObj in _shapes)
        {
            Destroy(persObj.gameObject);
        }

        _shapes.Clear();
    }

    private void SaveGame()
    {
        _storage.Save(this);
    }

    private void LoadGame()
    {
        StartNewGame();

        _storage.Load(this);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(saveVersion);

        writer.Write(_shapes.Count);
        foreach (var shape in _shapes)
        {
            if (saveVersion > 1)
            {
                writer.Write(shape.ShapeID);
            }
            writer.Write(shape.MaterialID);
            shape.Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int fileVersion = reader.ReadInt();

        if (fileVersion > saveVersion)
        {
            Debug.LogError("Unsupported file version " + fileVersion);
            return;
        }

        int count = reader.ReadInt();
        for (int i = 0; i < count; ++i)
        {
            Shape shapeInstance = _shapeFactory.Get(reader.ReadInt(), ((fileVersion > 1) ? (reader.ReadInt()) : (0)));
            shapeInstance.Load(reader);
            _shapes.Add(shapeInstance);
        }
    }
}
