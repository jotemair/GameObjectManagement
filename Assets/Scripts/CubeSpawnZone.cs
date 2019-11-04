using UnityEngine;

public class CubeSpawnZone : SpawnZone
{
    [SerializeField]
    private bool _surfaceOnly = false;

    public override Vector3 SpawnPoint
    {
        get
        {
            int selectedSurface = _surfaceOnly ? Random.Range(0, 3) : -1;

            Vector3 point = Vector3.zero;
            point.x = ( (0 == selectedSurface) ? (Random.Range(0, 2) == 0 ? 0.5f : -0.5f) : Random.Range(-0.5f, 0.5f) );
            point.y = ( (1 == selectedSurface) ? (Random.Range(0, 2) == 0 ? 0.5f : -0.5f) : Random.Range(-0.5f, 0.5f) );
            point.z = ( (2 == selectedSurface) ? (Random.Range(0, 2) == 0 ? 0.5f : -0.5f) : Random.Range(-0.5f, 0.5f) );

            return transform.TransformPoint(point);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
