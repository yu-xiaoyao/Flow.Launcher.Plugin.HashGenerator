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


    public static string CalcMd5_32(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = MD5.HashData(content);
        Console.WriteLine($"{hash.Length}");
        return ToHashString(hash, format);
    }

    public static string CalcMd5_16(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = MD5.HashData(content);
        hash = hash.Skip(4).Take(8).ToArray();
        return ToHashString(hash, format);
    }

    public static string CalcSha1(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = SHA1.HashData(content);
        return ToHashString(hash, format);
    }

    public static string CalcSha256(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = SHA256.HashData(content);
        return ToHashString(hash, format);
    }

    public static string CalcSha384(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = SHA384.HashData(content);
        return ToHashString(hash, format);
    }

    public static string CalcSha512(byte[] content, HashResultFormat format = HashResultFormat.Hex)
    {
        var hash = SHA512.HashData(content);
        return ToHashString(hash, format);
    }

    private static string ToHashString(byte[] hash, HashResultFormat format)
    {
        return format switch
        {
            HashResultFormat.Base64 => Convert.ToBase64String(hash),
            HashResultFormat.Hex_Upper => BitConverter.ToString(hash).Replace("-", ""),
            _ => BitConverter.ToString(hash).Replace("-", "").ToLower()
        };
    }
}