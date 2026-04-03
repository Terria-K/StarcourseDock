using System.IO.Compression;
using System.Text;

namespace Teuria.OnlineCoOp;

public static class JsonCompression
{
    public const int ChunkSize = 1000;

    public static byte[] Compress(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        using var output = new MemoryStream();

        using (var gzip = new GZipStream(output, CompressionMode.Compress))
        {
            gzip.Write(bytes, 0, bytes.Length);
        }

        return output.ToArray();
    }

    public static string Decompress(byte[] compressedJson)
    {
        using var input = new MemoryStream(compressedJson);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();

        gzip.CopyTo(output);

        return Encoding.UTF8.GetString(output.ToArray());
    }

    public static IEnumerable<string> Split(string str)
    {
        for (int i = 0; i < str.Length; i += ChunkSize)
            yield return str.Substring(i, Math.Min(ChunkSize, str.Length - i));
    }
}
