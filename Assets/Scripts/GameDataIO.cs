using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataWriter
{
    private BinaryWriter _writer = null;

    public GameDataWriter(BinaryWriter writer)
    {
        _writer = writer;
    }

    public void Write(int data)
    {
        _writer.Write(data);
    }

    public void Write(float data)
    {
        _writer.Write(data);
    }

    public void Write(Vector3 data)
    {
        _writer.Write(data.x);
        _writer.Write(data.y);
        _writer.Write(data.z);
    }

    public void Write(Quaternion data)
    {
        _writer.Write(data.x);
        _writer.Write(data.y);
        _writer.Write(data.z);
        _writer.Write(data.w);
    }

    public void Write(Color data)
    {
        _writer.Write(data.r);
        _writer.Write(data.g);
        _writer.Write(data.b);
        _writer.Write(data.a);
    }
}

public class GameDataReader
{
    private BinaryReader _reader = null;

    public GameDataReader(BinaryReader reader)
    {
        _reader = reader;
    }

    public int ReadInt()
    {
        return _reader.ReadInt32();
    }

    public float ReadFloat()
    {
        return _reader.ReadSingle();
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Quaternion ReadQuaternion()
    {
        return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Color ReadColor()
    {
        return new Color(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }
}