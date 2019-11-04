using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField]
    SpawnZone _spawnZone = null;

    void Start()
    {
        Game.Instance.SpawnZoneOfLevel = _spawnZone;
    }
}
