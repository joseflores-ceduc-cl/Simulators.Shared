using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public static class AesHelper
{
    // Derivar clave con SHA-256 (32 bytes)
    private static byte[] DeriveKey(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    // -------------------------------------------------------
    // Cifrar
    // -------------------------------------------------------
    public static string Encrypt(string plainText, string password)
    {
        byte[] key = DeriveKey(password);

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.GenerateIV(); // IV aleatorio

            using (var ms = new MemoryStream())
            {
                // Escribimos IV al inicio del mensaje
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // -------------------------------------------------------
    // Descifrar
    // -------------------------------------------------------
    public static string Decrypt(string cipherTextBase64, string password)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherTextBase64);
        byte[] key = DeriveKey(password);

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Key = key;

            int ivSize = aes.BlockSize / 8;
            byte[] iv = new byte[ivSize];

            Array.Copy(fullCipher, 0, iv, 0, ivSize);
            aes.IV = iv;

            using (var ms = new MemoryStream(fullCipher, ivSize, fullCipher.Length - ivSize))
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
