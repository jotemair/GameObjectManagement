﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    // Version number to manage changes to how we read and write information to save files
    const int saveVersion = 4;

    [SerializeField]
    public ShapeFactory _shapeFactory = null;

    // Input keys

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

    // Reference to storage
    [SerializeField]
    private PersistentStorage _storage = null;

    // Number of level scenes
    [SerializeField]
    private int _levelCount = 5;

    private int _currentLevel = -1;

    // Keeping track of the random state to allow saving/loading the state of the random number generator
    private Random.State _mainRandomState = default;

    // Automatic creation and destruction speed

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

    // Reference to spawnzone
    [SerializeField]
    private SpawnZone _spawnZone = null;

    public SpawnZone SpawnZoneOfLevel
    {
        get { return _spawnZone; }
        set { _spawnZone = value; }
    }

    // Singleton like access
    private static Game _instance = null;

    public static Game Instance
    {
        get { return _instance; }
    }

    private void Start()
    {
        _mainRandomState = Random.state;

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    _currentLevel = int.Parse(loadedScene.name.Substring(loadedScene.name.Length - 1));
                    return;
                }
            }
        }

        StartNewGame();
        StartCoroutine(LoadLevel(1));
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
        else
        {
            for (int i = 1; i <= _levelCount; ++i)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    StartNewGame();
                    StartCoroutine(LoadLevel(i));
                }
            }
        }
    }

    // Create a new random shape
    private void CreateShape()
    {
        Shape shapeInstance = _shapeFactory.GetRandom();
        Transform trans = shapeInstance.transform;
        trans.position = _spawnZone.SpawnPoint;
        trans.rotation = Random.rotation;
        trans.localScale = Vector3.one * Random.Range(0.1f, 1f);

        _shapes.Add(shapeInstance);
    }

    // Destroy a randomly selected shape instance
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

    // Clear the scene and start a new game
    private void StartNewGame()
    {
        Random.state = _mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        _mainRandomState = Random.state;
        Random.InitState(seed);

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

    // Save game data
    public override void Save(GameDataWriter writer)
    {
        writer.Write(saveVersion);

        writer.Write(_shapes.Count);
        writer.Write(Random.state);
        writer.Write(_currentLevel);
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

    // Load game data
    public override void Load(GameDataReader reader)
    {
        int fileVersion = reader.ReadInt();

        if (fileVersion > saveVersion)
        {
            Debug.LogError("Unsupported file version " + fileVersion);
            return;
        }

        int count = reader.ReadInt();

        Random.State randomState = default;
        if (saveVersion > 3)
        {
            randomState = reader.ReadRandomState();
        }

        StartCoroutine(LoadLevel((saveVersion < 3) ? 1 : reader.ReadInt()));

        for (int i = 0; i < count; ++i)
        {
            Shape shapeInstance = _shapeFactory.Get(reader.ReadInt(), ((fileVersion > 1) ? (reader.ReadInt()) : (0)));
            shapeInstance.Load(reader);
            _shapes.Add(shapeInstance);
        }

        if (saveVersion > 3)
        {
            Random.state = randomState;
        }
    }

    // Load level scene
    private IEnumerator LoadLevel(int idx)
    {
        if (_currentLevel != idx)
        {
            if (-1 != _currentLevel)
            {
                yield return SceneManager.UnloadSceneAsync(_currentLevel);
            }

            string levelName = "Level " + idx.ToString();

            enabled = false;
            yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
            enabled = true;

            _currentLevel = idx;
        }

        yield return null;
    }
}
