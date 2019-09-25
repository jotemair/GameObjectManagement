using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    private int _shapeID = int.MinValue;

    public int MaterialID { get; private set; } = int.MinValue;

    private Color _color = Color.white;

    public int ShapeID
    {
        get
        {
            return _shapeID;
        }

        set
        {
            if ((int.MinValue == _shapeID) && (int.MinValue != value))
            {
                _shapeID = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeID.");
            }
        }
    }

    public void SetMaterial(Material material, int materialId)
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialID = materialId;
    }

    public void SetColor(Color color)
    {
        _color = color;
        GetComponent<MeshRenderer>().material.color = color;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(_color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.ReadColor());
    }
}
