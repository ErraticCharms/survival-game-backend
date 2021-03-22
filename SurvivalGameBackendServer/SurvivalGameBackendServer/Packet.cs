using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Packet : IDisposable
{
    public List<byte> bytes = new List<byte>();

    public int length;

    public int readPos = 0;
    public void WriteLength()
    {
        length = bytes.Count;
    }

    public void WriteBytes(byte[] data)
    {
        foreach (byte newByte in data)
        {
            bytes.Add(newByte);
        }
    }

    public void WriteAtStart(byte[] data)
    {
        foreach (byte newbyte in data)
        {
            bytes.Insert(0, newbyte);
        }
    }

    public void WriteInt(int value)
    {
        byte[] intToWrite = BitConverter.GetBytes(value);

        foreach (byte byteToWrite in intToWrite)
        {
            bytes.Add(byteToWrite);
        }
    }

    public int ReadInt()
    {
        byte[] readInt;

        readInt = bytes.Skip(readPos).Take(4).ToArray();

        readPos += 4;

        return BitConverter.ToInt32(readInt, 0);
    }

    public void WriteFloat(float value)
    {
        byte[] floatToWrite = BitConverter.GetBytes(value);

        foreach (byte byteToWrite in floatToWrite)
        {
            bytes.Add(byteToWrite);
        }
    }

    public float ReadFloat()
    {
        byte[] readFloat;

        readFloat = bytes.Skip(readPos).Take(4).ToArray();

        readPos += 4;

        return BitConverter.ToSingle(readFloat, 0);
    }

    public void WriteString(string value)
    {
        byte[] stringBytes = Encoding.ASCII.GetBytes(value.ToCharArray());

        WriteInt(stringBytes.Length);

        foreach (byte byteToAdd in stringBytes)
        {
            bytes.Add(byteToAdd);
        }
    }

    public string ReadString()
    {
        int stringLength = ReadInt();

        byte[] stringBytes = bytes.Skip(readPos).Take(stringLength).ToArray();

        readPos += stringLength;

        return Encoding.ASCII.GetString(stringBytes);
    }

    public void WriteBool(bool value)
    {
        foreach (byte byteToAdd in BitConverter.GetBytes(value))
        {
            bytes.Add(byteToAdd);
        }
    }

    public bool ReadBool()
    {
        byte[] readBool = bytes.Skip(readPos).Take(1).ToArray();

        readPos++;
        return BitConverter.ToBoolean(readBool, 0);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
