using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using UnityEngine;

public class CryptoString
{
    private string encryptedValue;

    public CryptoString(CryptoString src)
    {
        this.encryptedValue = string.Format("{0}", src.encryptedValue);
    }

    public static implicit operator CryptoString(string i)
    {
        return new CryptoString(i);
    }
}

public class CryptoHelper
{
    private const int MaxReadBufferSize = 1024 * 10;
    private static SHA256Managed sha256 = new SHA256Managed();
    private static byte[] key;
    private static byte[] iv;
    private static Aes aes;
    private static ICryptoTransform cryptoTransform;
    private static byte[] buffer = new byte[2048];

    private static ICryptoTransform GetRijndaelEncryptor(string password)
    {
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] iv = new byte[16];
        Array.Copy(key, iv, iv.Length);

        RijndaelManaged rijndael = new RijndaelManaged();
        return rijndael.CreateEncryptor(key, iv);
    }

    private static ICryptoTransform GetRijndaelDecryptor(string password)
    {
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] iv = new byte[16];
        Array.Copy(key, iv, iv.Length);

        RijndaelManaged rijndael = new RijndaelManaged();
        return rijndael.CreateDecryptor(key, iv);
    }

    private static ICryptoTransform GetAesEncryptor(string password)
    {
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] iv = new byte[16];
        Array.Copy(key, iv, iv.Length);

        Aes aes = Aes.Create();
        return aes.CreateEncryptor(key, iv);
    }

    public static ICryptoTransform GetAesDecryptor(string password)
    {
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] iv = new byte[16];
        Array.Copy(key, iv, iv.Length);

        Aes aes = Aes.Create();
        return aes.CreateDecryptor(key, iv);
    }

    public static ICryptoTransform GetAesDecryptor2(string password)
    {
        if (cryptoTransform != null)
            return cryptoTransform;

        key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        iv = new byte[16];
        Array.Copy(key, iv, iv.Length);

        aes = Aes.Create();
        cryptoTransform = aes.CreateDecryptor(key, iv);
        return cryptoTransform;
    }

    public static string Encrypt(string text)
    {
        return Encrypt("pfproject", text);
    }

    public static byte[] Encrypt(byte[] bytes)
    {
        return Encrypt("pfproject", bytes);
    }

    public static string Decrypt(string crypted)
    {
        return Decrypt("pfproject", crypted);
    }

    public static byte[] Decrypt(byte[] crypted)
    {
        return Decrypt("pfproject", crypted);
    }

    public static string Decrypt(string password, string crypted)
    {
        var decryptor = GetAesDecryptor(password);

        byte[] cryptedData = Convert.FromBase64String(crypted);
        string decrypted = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(cryptedData, 0, cryptedData.Length));
        return decrypted;
    }

    public static byte[] Decrypt(string password, byte[] crypted)
    {
        var decryptor = GetAesDecryptor(password);
        return decryptor.TransformFinalBlock(crypted, 0, crypted.Length);
    }

    public static byte[] Decrypt2(string password, byte[] crypted)
    {
        var decryptor = GetAesDecryptor2(password);
        return decryptor.TransformFinalBlock(crypted, 0, crypted.Length);
    }

    public static string Encrypt(string password, string text)
    {
        var encryptor = CryptoHelper.GetAesEncryptor(password);

        byte[] textData = Encoding.UTF8.GetBytes(text);
        string encrypted = Convert.ToBase64String(encryptor.TransformFinalBlock(textData, 0, textData.Length));
        return encrypted;
    }

    public static byte[] Encrypt(string password, byte[] bytes)
    {
        var encryptor = CryptoHelper.GetAesEncryptor(password);
        return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    public static byte[] GetHashValue(string text)
    {
        return GetHashValue(Encoding.UTF8.GetBytes(text));
    }

    public static string GetHashValueString(string text)
    {
        var hashBytes = GetHashValue(text);
        var hex = BytesToHexString(hashBytes);
        return hex;
    }

    public static byte[] GetHashValue(byte[] bytes)
    {
        byte[] hashValue = sha256.ComputeHash(bytes);
        return hashValue;
    }

    public static string GetHashValueString(byte[] bytes)
    {
        return GetHashValueString(bytes, 0, bytes.Length);
    }

    public static string GetHashValueString(byte[] bytes, int offset, int count)
    {
        var hashBytes = GetHashValue(bytes);
        var hex = BytesToHexString(hashBytes);
        return hex;
    }

    public static string GetHashFromFile(string path)
    {
        string ret = null;

        try
        {
            if (!File.Exists(path))
            {
                DebugEx.LogError($"[Failed] A file does not exist, path: {path}");
                return null;
            }

            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var outStream = new MemoryStream();
                var buf = new byte[MaxReadBufferSize];
                int readLen = 0;
                while ((readLen = stream.Read(buf, 0, buf.Length)) > 0)
                {
                    outStream.Write(buf, 0, readLen);
                }
                outStream.Close();
                byte[] bin = outStream.ToArray();
                stream.Close();

                ret = BytesToHexString(GetHashValue(bin));
            }
        }
        catch (Exception e)
        {
            ret = null;
            DebugEx.LogError($"[Failed] can't make hash from a file, path: {path}\n{e.Message}");
        }

        return ret;
    }

    public static string BytesToHexString(byte[] byteArray)
    {
        StringBuilder sb = new StringBuilder(byteArray.Length);
        for (int i = 0; i < byteArray.Length; i++)
        {
            sb.Append(byteArray[i].ToString("x2"));
        }
        return sb.ToString();
    }

}
