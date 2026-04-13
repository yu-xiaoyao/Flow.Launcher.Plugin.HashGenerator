using System;
using System.Linq;
using System.Security.Cryptography;

namespace Flow.Launcher.Plugin.HashGenerator;

public class HashHelper
{
    public enum HashResultFormat
    {
        Hex, // hex 小写
        Hex_Upper, // hex 大写
        Base64
    }


    public static byte[] CalcMd5_32(byte[] content)
    {
        var hash = MD5.HashData(content);
        return hash;
    }

    public static byte[] CalcMd5_16(byte[] content)
    {
        var hash = MD5.HashData(content);
        hash = hash.Skip(4).Take(8).ToArray();
        return hash;
    }

    public static byte[] CalcSha1(byte[] content)
    {
        return SHA1.HashData(content);
    }

    public static byte[] CalcSha256(byte[] content)
    {
        return SHA256.HashData(content);
    }

    public static byte[] CalcSha384(byte[] content)
    {
        return SHA384.HashData(content);
    }

    public static byte[] CalcSha512(byte[] content)
    {
        return SHA512.HashData(content);
    }

    public static string ToHashString(byte[] hash, HashResultFormat format)
    {
        return format switch
        {
            HashResultFormat.Base64 => Convert.ToBase64String(hash),
            HashResultFormat.Hex_Upper => BitConverter.ToString(hash).Replace("-", ""),
            _ => BitConverter.ToString(hash).Replace("-", "").ToLower()
        };
    }
}