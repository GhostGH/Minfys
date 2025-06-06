using NAudio.Wave;

namespace Minfys.Models;

public class LoopStream : WaveStream
{
    private readonly WaveStream sourceStream;
    public bool EnableLooping { get; set; }

    public LoopStream(WaveStream sourceStream)
    {
        this.sourceStream = sourceStream;
        this.EnableLooping = true;
    }

    /// <summary>
    /// Return source stream's wave format
    /// </summary>
    public override WaveFormat WaveFormat
    {
        get { return sourceStream.WaveFormat; }
    }

    /// <summary>
    /// LoopStream simply returns
    /// </summary>
    public override long Length
    {
        get { return sourceStream.Length; }
    }

    public override long Position
    {
        get => sourceStream.Position;
        set => sourceStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;
        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (!EnableLooping || sourceStream.Position == 0)
                {
                    break;
                }

                sourceStream.Position = 0;
                //continue;
            }

            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }
}