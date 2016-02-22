using System.IO.Compression;

/// <summary>
/// Class to allow for extraction and compression of an entire directory.
/// </summary>

public class Compression
{
    public static void Decompress(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }

    public static void Compress(string compressPath, string zipPath)
    {
        ZipFile.CreateFromDirectory(compressPath, zipPath, CompressionLevel.Fastest, true);
    }
}