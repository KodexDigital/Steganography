using System.Security.Cryptography;
using System.Text;

namespace Steganography.Helpers
{
    public static class CryptoHelper
    {
        public static string Encrypt(string text, string password)
        {
            DeriveKeyAndIV(password, out byte[] key, out byte[] iv);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encryptedBase64, string password)
        {
            DeriveKeyAndIV(password, out byte[] key, out byte[] iv);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] encrypted = Convert.FromBase64String(encryptedBase64);
            byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static void DeriveKeyAndIV(string password, out byte[] key, out byte[] iv)
        {
            using var sha256 = SHA256.Create();
            byte[] pwBytes = Encoding.UTF8.GetBytes(password);
            key = sha256.ComputeHash(pwBytes); // 32 bytes = 256-bit key
            iv = new byte[16]; // 128-bit IV (just zeros for simplicity here)
        }
    }
}