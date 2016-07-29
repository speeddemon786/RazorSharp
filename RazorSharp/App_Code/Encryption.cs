using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// class for encrypting and decrypting data
/// </summary>

public class Encryption
{
    public static string GetEncryptionKey()
    {
        return ConfigurationManager.AppSettings["EncryptionKey"];
    }

    public static string Encrypt(string clearText)
    {
        var encryptionKey = GetEncryptionKey();
        var clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (var encryptor = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0X49, 0X76, 0X61, 0X6E, 0X20, 0X4D, 0X65, 0X64, 0X76, 0X65, 0X64, 0X65, 0X76 });
            if (encryptor == null) return clearText;
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText)
    {
        var encryptionKey = GetEncryptionKey();
        var cipherBytes = Convert.FromBase64String(cipherText);
        using (var encryptor = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0X49, 0X76, 0X61, 0X6E, 0X20, 0X4D, 0X65, 0X64, 0X76, 0X65, 0X64, 0X65, 0X76 });
            if (encryptor == null) return cipherText;
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
}