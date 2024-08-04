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

    private static readonly bool DBG = true;

    public static byte[] AesEncrypt(byte[] source, byte[] aesKeyArray)
    {
        byte[] encryptResArray = null;
        try
        {
            // LogUtil.D("aesKey:" + BitConverter.ToString(aesKeyArray).Replace("-", ""), true);
            encryptResArray = Encrypt(source, aesKeyArray, aesKeyArray);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // LogUtil.D("encryptResArray=" + BitConverter.ToString(encryptResArray).Replace("-", ""), DBG);
        return encryptResArray;
    }

    public static byte[] AesDecrypt(byte[] source, byte[] aesKeyArray)
    {
        byte[] decryptResArray = null;
        try
        {
            // LogUtil.D("aesKey:" + BitConverter.ToString(aesKeyArray).Replace("-", ""), true);
            decryptResArray = Decrypt(source, aesKeyArray, aesKeyArray);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return decryptResArray;
    }

    private static byte[] Encrypt(byte[] source, byte[] key, byte[] iv)
    {
        if (key == null)
        {
            LogUtilW("Key is null", DBG);
            return null;
        }

        if (key.Length != 16)
        {
            LogUtilW("the length of Key is not 16", DBG);
            return null;
        }

        if (iv.Length != 16)
        {
            LogUtilW("the length of IV vector is not 16", DBG);
            return null;
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                return PerformCryptography(source, encryptor);
            }
        }
    }

    private static byte[] Decrypt(byte[] source, byte[] key, byte[] iv)
    {
        if (key == null)
        {
            LogUtilE("Key is null", DBG);
            return null;
        }

        if (key.Length != 16)
        {
            LogUtilE("the length of Key is not 16", DBG);
            return null;
        }

        if (iv.Length != 16)
        {
            LogUtilE("the length of IV vector is not 16", DBG);
            return null;
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                try
                {
                    return PerformCryptography(source, decryptor);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LogUtilW("source=" + BitConverter.ToString(source).Replace("-", ""), DBG);
                    LogUtilW("key=" + BitConverter.ToString(key).Replace("-", ""), DBG);
                    return null;
                }
            }
        }
    }

    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }
    }

    private static void LogUtilW(string message, bool debug)
    {
        if (debug)
        {
            Console.WriteLine("Warning: " + message);
        }
    }

    private static void LogUtilE(string message, bool debug)
    {
        if (debug)
        {
            Console.WriteLine("Error: " + message);
        }
    }
}