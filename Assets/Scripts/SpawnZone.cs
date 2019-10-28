using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField]
    private bool _surfaceOnly = false;

    public Vector3 SpawnPoint
    {
        get { return transform.TransformPoint(_surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}
