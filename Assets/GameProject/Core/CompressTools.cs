using System;
using System.IO;
using System.IO.Compression;

public static class CompressTools
{
    /// <summary>
    /// 压缩字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string CompressString(string str, string encoding = "UTF-8")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        string result = string.Empty;
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream decompressedStream = new GZipStream(ms, CompressionMode.Compress))
            {
                decompressedStream.Write(bytes, 0, bytes.Length);
                decompressedStream.Close();
                result = Convert.ToBase64String(ms.ToArray());
            }
        }
        return result;
    }

    /// <summary>
    /// 解压字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string DecompressString(string str, string encoding = "UTF-8")
    {
        var bytes = Convert.FromBase64String(str);
        string result = string.Empty;

        using (MemoryStream tempMs = new MemoryStream())
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (GZipStream Decompress = new GZipStream(ms, CompressionMode.Decompress))
                {
                    Decompress.CopyTo(tempMs);
                    result = System.Text.Encoding.UTF8.GetString(tempMs.ToArray());
                }
            }
        }
        return result;
    }
}
