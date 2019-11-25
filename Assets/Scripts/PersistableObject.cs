using UnityEngine;

// We only one a single persisting component on a GameObject
[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    public virtual void Save(GameDataWriter writer)
    {
        // Write the position, rotation and scale to file
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }

    public virtual void Load(GameDataReader reader)
    {
        // Read the position, rotation and scale from file
        transform.localPosition = reader.ReadVector3();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector3();
    }
}
