using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;

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
        string EncryptionKey = GetEncryptionKey();
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0X49, 0X76, 0X61, 0X6E, 0X20, 0X4D, 0X65, 0X64, 0X76, 0X65, 0X64, 0X65, 0X76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
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
        string EncryptionKey = GetEncryptionKey();
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0X49, 0X76, 0X61, 0X6E, 0X20, 0X4D, 0X65, 0X64, 0X76, 0X65, 0X64, 0X65, 0X76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
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