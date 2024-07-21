namespace Shared;
using System;
using System.Security.Cryptography;
using System.Text;

public static class AESUtil
{
    private static readonly byte[] DefaultAESKey = new byte[]
    {
        0x98, 0x76, 0x23, 0xE8,
        0xA9, 0x23, 0xA1, 0xBB,
        0x3D, 0x9E, 0x7D, 0x03,
        0x78, 0x12, 0x45, 0x88
    };

    public static byte[] AESEncrypt(byte[] source, byte[]? key = null)
    {
        if (source.Length == 0)
        {
            return new byte[0];
        }

        key ??= DefaultAESKey;

        if (key.Length != 16)
        {
            throw new ArgumentException("Invalid key size: " + key.Length);
        }

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = key;

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return PerformCryptography(source, encryptor);
    }

    public static byte[] AESDecrypt(byte[] source, byte[]? key = null)
    {
        if (source.Length == 0)
        {
            return new byte[0];
        }

        key ??= DefaultAESKey;

        if (key.Length != 16)
        {
            throw new ArgumentException("Invalid key size: " + key.Length);
        }

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = key;

        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        try
        {
            return PerformCryptography(source, decryptor);
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine(ex);
            throw new Exception("Decryption failed");
        }
    }

    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();
        return memoryStream.ToArray();
    }
}
