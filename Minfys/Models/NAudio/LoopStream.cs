using NAudio.Wave;

namespace Minfys.Models.NAudio;

public class LoopStream : WaveStream
{
    private readonly WaveStream _sourceStream;
    public bool EnableLooping { get; set; }

    public LoopStream(WaveStream sourceStream)
    {
        this._sourceStream = sourceStream;
        this.EnableLooping = true;
    }

    public override WaveFormat WaveFormat
    {
        get { return _sourceStream.WaveFormat; }
    }

    public override long Length
    {
        get { return _sourceStream.Length; }
    }

    public override long Position
    {
        get => _sourceStream.Position;
        set => _sourceStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;
        while (totalBytesRead < count)
        {
            int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (!EnableLooping || _sourceStream.Position == 0)
                {
                    break;
                }

                _sourceStream.Position = 0;
            }

            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }
}