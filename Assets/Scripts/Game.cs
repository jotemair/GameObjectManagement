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

    [SerializeField]
    private KeyCode _destroyKey = KeyCode.X;

    private List<Shape> _shapes = new List<Shape>();

    [SerializeField]
    private PersistentStorage _storage = null;

    [SerializeField]
    private float _creationSpeed = 0f;

    public float CreationSpeed
    {
        get { return _creationSpeed; }
        set { _creationSpeed = value; }
    }

    private float _creationTimer = 0f;

    [SerializeField]
    private float _destructionSpeed = 0f;

    public float DestructionSpeed {
        get { return _destructionSpeed; }
        set { _destructionSpeed = value; }
    }

    private float _destructionTimer = 0f;

    [SerializeField]
    private SpawnZone _spawnZone = null;

    public SpawnZone SpawnZoneOfLevel
    {
        get { return _spawnZone; }
        set { _spawnZone = value; }
    }

    private static Game _instance = null;

    public Game Instance
    {
        get { return _instance; }
    }

    private void OnEnable()
    {
        _instance = this;
    }

    private void Update()
    {
        _creationTimer += Time.deltaTime * _creationSpeed;
        while (_creationTimer >= 1f)
        {
            _creationTimer -= 1f;
            CreateShape();
        }

        _destructionTimer += Time.deltaTime * _destructionSpeed;
        while (_destructionTimer >= 1f)
        {
            _destructionTimer -= 1f;
            DestroyShape();
        }

        if (Input.GetKeyDown(_createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(_destroyKey))
        {
            DestroyShape();
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
        trans.position = _spawnZone.SpawnPoint;
        trans.rotation = Random.rotation;
        trans.localScale = Vector3.one * Random.Range(0.1f, 1f);

        _shapes.Add(shapeInstance);
    }

    private void DestroyShape()
    {
        if (_shapes.Count > 0)
        {
            int idx = Random.Range(0, _shapes.Count);
            Shape shapeToDestroy = _shapes[idx];
            _shapes[idx] = _shapes[_shapes.Count - 1];
            _shapes.RemoveAt(_shapes.Count - 1);
            _shapeFactory.Reclaim(shapeToDestroy);
        }
    }

    private void StartNewGame()
    {
        foreach (var persObj in _shapes)
        {
            _shapeFactory.Reclaim(persObj);
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
