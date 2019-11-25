using UnityEngine;

public class Shape : PersistableObject
{
    private static int ColorPropertyId = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock _sharedPropertyBlock = null;

    private static MaterialPropertyBlock SharedPropertyBlock
    {
        get
        {
            if (null == _sharedPropertyBlock)
            {
                _sharedPropertyBlock = new MaterialPropertyBlock();
            }

            return _sharedPropertyBlock;
        }
    }

    private int _shapeID = int.MinValue;

    public int MaterialID { get; private set; } = int.MinValue;

    private Color _color = Color.white;

    private Renderer _renderer = null;

    private Renderer MeshRenderer
    {
        get
        {
            if (null == _renderer)
            {
                _renderer = GetComponent<MeshRenderer>();
            }

            return _renderer;
        }
    }

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

    // Set the material of the shape
    public void SetMaterial(Material material, int materialId)
    {
        MeshRenderer.material = material;
        MaterialID = materialId;
    }

    // Set the color of the shape
    public void SetColor(Color color)
    {
        _color = color;
        SharedPropertyBlock.SetColor(ColorPropertyId, _color);
        MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
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
