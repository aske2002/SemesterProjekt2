public class BinaryAdapter : IDisposable
{
    private MemoryStream _stream;
    private BinaryReader _reader;
    private BinaryWriter _writer;
    public BinaryAdapter(MemoryStream stream)
    {
        _stream = stream;
        _reader = new BinaryReader(_stream);
        _writer = new BinaryWriter(_stream);
    }

    public BinaryReader Reader => _reader;
    public BinaryWriter Writer => _writer;
    public long RemainingBytes => _stream.CanSeek ? _stream.Length - _stream.Position : -1;
    public bool IsEmpty => RemainingBytes == 0;
    public long Position => _stream.Position;

    public byte[] ToArray()
    {

        return _stream.ToArray();
    }

    public void Write(byte[] data)
    {
        _writer.Write(data);
    }

    public void Write(byte data)
    {
        _writer.Write(data);
    }

    public void Write(int data)
    {
        _writer.Write(data);
    }

    public void Write(double data)
    {
        _writer.Write(data);
    }

    public void Write(string data)
    {
        _writer.Write(data);
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public int ReadInt32()
    {
        return _reader.ReadInt32();
    }

    public double ReadDouble()
    {
        return _reader.ReadDouble();
    }

    public string ReadString()
    {
        return _reader.ReadString();
    }

    public Guid ReadGuid()
    {
        return new Guid(_reader.ReadBytes(16));
    }

    public byte[] ReadBytes(int length)
    {
        return _reader.ReadBytes(length);
    }

    public byte[] ReadAll()
    {
        return _reader.ReadBytes((int)RemainingBytes);
    }

    public string ReadAllAsString()
    {
        var bytes = ReadAll();
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public static BinaryAdapter Empty()
    {
        var stream = new MemoryStream();
        return new BinaryAdapter(stream);
    }

    public BinaryAdapter CreateSubReader(int length)
    {
        var data = _reader.ReadBytes(length);
        var stream = new MemoryStream(data);
        return new BinaryAdapter(stream);
    }
    public static BinaryAdapter Create(byte[] data)
    {
        var stream = new MemoryStream(data);
        return new BinaryAdapter(stream);
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _writer?.Dispose();
        _stream?.Dispose();
    }
}