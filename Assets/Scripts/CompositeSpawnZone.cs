using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    List<SpawnZone> spawnZones = new List<SpawnZone>();

    public override Vector3 SpawnPoint
    {
        get
        {
            // Return a spawnpoint from a randomly selected SpawnZone from the spawnZones list
            return spawnZones[Random.Range(0, spawnZones.Count)].SpawnPoint;
        }
    }
}
