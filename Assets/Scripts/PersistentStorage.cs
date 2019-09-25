using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    private string _savePath;

    void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject obj)
    {
        // The using (something) {} format will dispose of the something after the code block even if an exception occurs
        // Basically it's shorthand for a try catch block, only works with IDisposable types
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create)))
        {
            obj.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject obj)
    {
        using (var reader = new BinaryReader(File.Open(_savePath, FileMode.Open)))
        {
            obj.Load(new GameDataReader(reader));
        }
    }
}
